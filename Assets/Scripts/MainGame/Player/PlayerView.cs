using System;
using MainGame.Stage;
using UniRx;
using UnityEngine;

namespace Sabanishi.MainGame
{
    public class PlayerView : MonoBehaviour
    {
        private static readonly int IsAir = Animator.StringToHash("IsAir");
        private static readonly int IsHang = Animator.StringToHash("IsHang");
        private static readonly int YSpeed = Animator.StringToHash("ySpeed");
        private static readonly int IsWalk = Animator.StringToHash("IsWalk");
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
            
            var hit = Physics2D.Raycast((Vector2)_transform.position + offset, direction, _rayLength,
                LayerMask.GetMask("Block"));
            
            if (hit.collider == null) return true;
            
            BlockChip chip = hit.collider.gameObject.GetComponent<BlockChip>();
            if (chip == null) return true;
            return !chip.CanStick(checkDirection);
        }

        /// <summary>
        /// 引数の方向にペンキが使える壁があるかを調べる
        /// </summary>
        public BlockChip CheckCanPaint(Direction bodyDirection,Direction checkDirection)
        {
            var offset = GetOffset(bodyDirection);

            BlockChip chip;
            
            chip = CheckCanPaint((Vector2)_transform.position + offset, checkDirection);
            if (chip != null) return chip;

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
            
            chip = CheckCanPaint((Vector2)_transform.position + offset + dir, checkDirection);
            if (chip != null) return chip;
            
            chip = CheckCanPaint((Vector2)_transform.position + offset - dir, checkDirection);
            if (chip != null) return chip;
                
            return null;
        }

        private BlockChip CheckCanPaint(Vector2 pos,Direction checkDirection)
        {
            var hit = Physics2D.Raycast(pos, ConvertToVector(checkDirection), _rayLength*2,
                LayerMask.GetMask("Block"));
            if(hit.collider == null) return null;
            
            var chip = hit.collider.gameObject.GetComponent<BlockChip>();
            if (chip == null) return null;
            
            if (chip.CanPaint(CalcUtils.ReverseDirection(checkDirection)))
            {
                return chip;
            }

            return null;
        }

        private Vector2 ConvertToVector(Direction dir)
        {
            switch (dir)
            {
                case Direction.Down:
                    return Vector2.down;
                case Direction.Left:
                    return Vector2.left;
                case Direction.Right:
                    return Vector2.right;
                case Direction.Up:
                    return Vector2.up;
                default:
                    return Vector2.zero;
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
    }
}