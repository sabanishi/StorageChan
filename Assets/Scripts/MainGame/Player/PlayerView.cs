using System;
using MainGame.Stage;
using UniRx;
using UnityEngine;

namespace Sabanishi.MainGame
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private GameObject _boxSprite;
        [SerializeField] private LayerMask _blockLayer;
        [SerializeField] private LayerMask _boxLayer;

        private static readonly int IsAir = Animator.StringToHash("IsAir");
        private static readonly int IsHang = Animator.StringToHash("IsHang");
        private static readonly int YSpeed = Animator.StringToHash("ySpeed");
        private static readonly int IsWalk = Animator.StringToHash("IsWalk");
        private static readonly int IsPaint = Animator.StringToHash("IsPaint");
        private static readonly int Paint = Animator.StringToHash("paint");
        private Animator _animator;
        private Transform _transform;
        private float _rayLength;
        private Vector2 _colliderOffset;
        private Vector2 _colliderSize;
        private Subject<Vector3> _posChangeSubject;
        public IObservable<Vector3> PosChangeSubject => _posChangeSubject;

        public void Initialize()
        {
            _transform = transform;
            _animator = GetComponent<Animator>();
            var collider = GetComponent<BoxCollider2D>();
            var size = collider.size;
            _rayLength = size.y / 2;
            _colliderOffset = collider.offset;
            _colliderSize = size;
            _posChangeSubject = new();
        }

        public void Dispose()
        {
            _posChangeSubject.Dispose();
        }

        public void OnPosChanged(Vector3 pos)
        {
            _transform.position = pos;
        }

        public void OnIsAirChanged(bool isAir)
        {
            _animator.SetBool(IsAir, isAir);
        }

        public void OnIsHangChanged(bool isHang)
        {
            _animator.SetBool(IsHang, isHang);
            _boxSprite.SetActive(isHang);
        }

        public void OnIsPaintModeChanged(bool isPaintMode)
        {
            _animator.SetBool(IsPaint, isPaintMode);
        }

        public void PlayPaintAnimation()
        {
            _animator.SetTrigger(Paint);
        }

        public void OnSpeedChanged(Vector3 vec)
        {
            _animator.SetFloat(YSpeed, vec.y);
            _animator.SetBool(IsWalk, vec.x != 0);
            if (vec.x > 0)
            {
                _transform.localScale = new Vector3(1, 1, 1);
            }
            else if (vec.x < 0)
            {
                _transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        public void OnBodyDirectionChanged(Direction bodyDirection)
        {
            float diff = (_colliderOffset.y + _colliderOffset.x) / 2;
            switch (bodyDirection)
            {
                case Direction.Down:
                    _transform.localEulerAngles = new Vector3(0, 0, 0);
                    break;
                case Direction.Up:
                    _transform.localEulerAngles = new Vector3(0, 0, 180);
                    break;
                case Direction.Right:
                    _transform.localEulerAngles = new Vector3(0, 0, 90);
                    _posChangeSubject.OnNext(_transform.position +
                                             Vector3.left * (_colliderSize.y - _colliderSize.x) / 2 +
                                             new Vector3(diff, diff, 0));
                    break;
                case Direction.Left:
                    _transform.localEulerAngles = new Vector3(0, 0, -90);
                    _posChangeSubject.OnNext(_transform.position +
                                             Vector3.right * (_colliderSize.y - _colliderSize.x) / 2 +
                                             new Vector3(-diff, diff, 0));
                    break;
            }
        }

        /// <summary>
        /// 足元をチェックして空中にいるかどうかを返す
        /// </summary>
        /// <returns></returns>
        public bool CheckIsAir(Direction bodyDirection)
        {
            //足元に何もない場合、空中にいると判定する
            Vector2 direction;
            Vector2 offset = GetOffset(bodyDirection);
            Direction checkDirection;

            switch (bodyDirection)
            {
                case Direction.Down:
                    direction = Vector2.down;
                    checkDirection = Direction.Up;
                    break;
                case Direction.Up:
                    direction = Vector2.up;
                    checkDirection = Direction.Down;
                    break;
                case Direction.Right:
                    direction = Vector2.right;
                    checkDirection = Direction.Left;
                    break;
                case Direction.Left:
                    direction = Vector2.left;
                    checkDirection = Direction.Right;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(bodyDirection), bodyDirection, null);
            }

            var hit = Physics2D.Raycast((Vector2)_transform.position + offset, direction, _rayLength, _blockLayer);

            if (hit.collider == null) return true;

            BlockChip chip = hit.collider.gameObject.GetComponent<BlockChip>();
            if (chip == null) return true;
            return !chip.CanStick(checkDirection);
        }

        /// <summary>
        /// 引数の方向にペンキが使える壁があるかを調べる
        /// </summary>
        public BlockChip CheckCanPaint(Direction bodyDirection, Direction checkDirection)
        {
            var offset = GetOffset(bodyDirection);

            BlockChip chip;

            chip = CheckCanPaint((Vector2)_transform.position + offset, checkDirection);
            if (chip != null) return chip;

            var dir = CalcCheckDirectionFootPos(bodyDirection, checkDirection);

            chip = CheckCanPaint((Vector2)_transform.position + offset + dir, checkDirection);
            if (chip != null) return chip;

            chip = CheckCanPaint((Vector2)_transform.position + offset - dir, checkDirection);
            if (chip != null) return chip;

            return null;
        }

        /// <summary>
        /// 体の向きと目的方向から、目的方向の足元の横の座標へのオフセットを返す
        /// </summary>
        private Vector2 CalcCheckDirectionFootPos(Direction bodyDirection, Direction checkDirection)
        {
            Vector2 dir;
            if (bodyDirection == Direction.Down || bodyDirection == Direction.Up)
            {
                if (checkDirection == Direction.Down || checkDirection == Direction.Up)
                {
                    dir = new Vector2(_colliderSize.x / 2.1f, 0);
                }
                else
                {
                    dir = new Vector2(0, _colliderSize.y / 2.1f);
                }
            }
            else
            {
                if (checkDirection == Direction.Down || checkDirection == Direction.Up)
                {
                    dir = new Vector2(_colliderSize.y / 2.1f, 0);
                }
                else
                {
                    dir = new Vector2(0, _colliderSize.x / 2.1f);
                }
            }

            return dir;
        }

        private BlockChip CheckCanPaint(Vector2 pos, Direction checkDirection)
        {
            var hit = Physics2D.Raycast(pos, ConvertToVector(checkDirection), _rayLength * 2,
                _blockLayer);
            if (hit.collider == null) return null;

            var chip = hit.collider.gameObject.GetComponent<BlockChip>();
            if (chip == null) return null;

            if (chip.CanPaint(CalcUtils.ReverseDirection(checkDirection)))
            {
                return chip;
            }

            return null;
        }

        private Vector2Int ConvertToVector(Direction dir)
        {
            switch (dir)
            {
                case Direction.Down:
                    return Vector2Int.down;
                case Direction.Left:
                    return Vector2Int.left;
                case Direction.Right:
                    return Vector2Int.right;
                case Direction.Up:
                    return Vector2Int.up;
                default:
                    return Vector2Int.zero;
            }
        }

        private Vector2 GetOffset(Direction bodyDirection)
        {
            switch (bodyDirection)
            {
                case Direction.Down:
                    return _colliderOffset;
                case Direction.Up:
                    return -_colliderOffset;
                case Direction.Right:
                    return new Vector2(-_colliderOffset.y, _colliderOffset.x);
                case Direction.Left:
                    return new Vector2(_colliderOffset.y, -_colliderOffset.x);
                default:
                    Debug.LogError("BodyDirectionが不正です");
                    return Vector2.zero;
            }
        }

        /// <summary>
        /// 目の前にあるハコを返す
        /// </summary>
        public Box CheckBox(Direction bodyDirection)
        {
            var offset = GetOffset(bodyDirection);
            var checkDirection = CalcCheckDirection(bodyDirection);

            var hit = Physics2D.Raycast((Vector2)_transform.position + offset, ConvertToVector(checkDirection),
                _rayLength * 2,
                _boxLayer);
            if (hit.collider == null) return null;
            var box = hit.collider.gameObject.GetComponent<Box>();
            if (box == null) return null;
            return box;
        }

        /// <summary>
        /// 目の前にハコを置けるかを返す
        /// </summary>
        public (bool, Vector2Int) CheckCanPutBox(Direction bodyDirection)
        {
            var offset = GetOffset(bodyDirection);
            var playerPos = (Vector2)_transform.position + offset;
            var checkPos = new Vector2Int(Mathf.RoundToInt(playerPos.x), Mathf.RoundToInt(playerPos.y));
            var checkDirection = CalcCheckDirection(bodyDirection);
            var hit = Physics2D.Raycast(checkPos, ConvertToVector(checkDirection),
                1.0f, _blockLayer);
            //目の前に何かあった場合はfalseを返す
            if (hit.collider != null) return (false, Vector2Int.zero);
            Vector2Int dir;
            if (bodyDirection == Direction.Down || bodyDirection == Direction.Up)
            {
                if (_transform.localScale.x == 1)
                {
                    dir = new Vector2Int(1, 0);
                }
                else
                {
                    dir = new Vector2Int(-1, 0);
                }
            }
            else
            {
                if (_transform.localScale.x == 1)
                {
                    dir = new Vector2Int(0, -1);
                }
                else
                {
                    dir = new Vector2Int(0, 1);
                }
            }

            hit = Physics2D.Raycast(checkPos + dir, ConvertToVector(bodyDirection),
                1.0f, _blockLayer);
            if (hit.collider == null) return (false, Vector2Int.zero);

            var chip = hit.collider.gameObject.GetComponent<BlockChip>();
            if (chip == null) return (false, Vector2Int.zero);

            var canPut = chip.CanStick(CalcUtils.ReverseDirection(bodyDirection));
            if (!canPut) return (false, Vector2Int.zero);

            var chipPos = chip.gameObject.transform.position;
            var pos = new Vector2Int((int)chipPos.x, (int)chipPos.y) - ConvertToVector(bodyDirection);
            return (true, pos);
        }

        private Direction CalcCheckDirection(Direction bodyDirection)
        {
            return _transform.localScale.x == 1
                ? CalcUtils.ReverseRotateDirection(bodyDirection)
                : CalcUtils.RotateDirection(bodyDirection);
        }
    }
}