using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MainGame.Stage;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;
using Box = MainGame.Stage.Box;

namespace Sabanishi.MainGame
{
    public class PlayerModel
    {
        private const float MoveSpeed = 5.0f;
        private const float GravityScale = 9.8f;
        private const float JumpPower = 5.0f;
        private const float _nigligibleError = 0.12f;

        #region ReactiveProperty

        private ReactiveProperty<bool> _isAir;
        public IReadOnlyReactiveProperty<bool> IsAir => _isAir;

        private Vector3 _beforePos; //1フレーム前の座標
        private readonly ReactiveProperty<Vector3> _pos;
        public IReadOnlyReactiveProperty<Vector3> Pos => _pos;
        private Vector3 _speedVec;

        private readonly ReactiveProperty<bool> _isHang;
        public IReadOnlyReactiveProperty<bool> IsHang => _isHang;

        private ReactiveProperty<Direction> _bodyDirection;
        public IReadOnlyReactiveProperty<Direction> NowBodyDirection => _bodyDirection;

        private ReactiveProperty<bool> _isPaintMode;
        public IObservable<bool> IsPaintMode => _isPaintMode;

        private Subject<Vector3> _onUpdateSpeedSubject;
        public IObservable<Vector3> OnUpdateSpeedSubject => _onUpdateSpeedSubject;
        private Subject<ChipData> _putBoxSubject;
        public IObservable<ChipData> PutBoxSubject => _putBoxSubject;
        private Subject<ChipData> _removeBoxSubject;
        public IObservable<ChipData> RemoveBoxSubject => _removeBoxSubject;

        #endregion

        private bool _canOperate;
        private BoxCollider2D _myCollider;
        private List<BoxCollider2D> _nearChipColliders;
        private bool _isRightCollide, _isLeftCollide, _isUpCollide, _isDownCollide;
        private bool _isStickJump; //地上以外からジャンプしたかどうか
        private BlockChip _nowSelectedChip;
        private bool _isPainted;
        private int _paintCount;
        public int PaintCount => _paintCount;

        public Func<Direction, Direction, BlockChip> CheckCanPaintAction;
        public Func<Direction, bool> CheckIsAirAction;
        public Func<Direction, Box> CheckBoxAction;
        public Func<Direction, (bool, Vector2Int)> CheckCanPutBox;
        public Action PlayPaintAction;

        public void SetCanOperate(bool canOperate)
        {
            _canOperate = canOperate;
        }

        public PlayerModel()
        {
            _isAir = new();
            _pos = new();
            _isHang = new();
            _bodyDirection = new();
            _isPaintMode = new();

            _nearChipColliders = new();
            _onUpdateSpeedSubject = new();
            _putBoxSubject = new();
            _removeBoxSubject = new();
        }

        public void Initialize(Vector3 startPos, BoxCollider2D myCollider)
        {
            _pos.Value = startPos;
            _isAir.Value = false;
            _isHang.Value = false;
            _bodyDirection.Value = Direction.Down;
            _isPaintMode.Value = false;

            _beforePos = startPos;
            _myCollider = myCollider;
            _speedVec = Vector3.zero;
            _canOperate = false;
            _paintCount = 0;
        }

        public void Dispose()
        {
            _isAir.Dispose();
            _pos.Dispose();
            _isHang.Dispose();
            _bodyDirection.Dispose();
            _isPaintMode.Dispose();

            _onUpdateSpeedSubject.Dispose();
            _putBoxSubject.Dispose();
            _removeBoxSubject.Dispose();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// 毎フレーム実行される処理
        /// </summary>
        public void Update(Vector3 beforePos, bool isAir)
        {
            //1フレームの間隔が大きすぎる時、処理を飛ばす
            if (Time.deltaTime > 0.2f) return;

            if (!isAir)
            {
                _isStickJump = false;
            }

            if (_isPainted)
            {
                if (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical"))
                {
                    _isPainted = false;
                }
            }

            _pos.Value = beforePos;
            _isAir.Value = isAir;
            if (_canOperate) InputPaint();
            if (_canOperate) InputHang();
            if (!_isPaintMode.Value) InputMove();
            ApplyGravity();
            if (!_isPaintMode.Value && _canOperate) InputJump();

            //移動処理
            _pos.Value += _speedVec * Time.deltaTime;

            ResolveCollision();
            SpeedUpdate();
            CheckNextStepIsAir(isAir);
            Rotate();

            if (!_canOperate) return;
            if (Input.GetButton("Paint"))
            {
                _isPaintMode.Value = true;
                _speedVec.x = 0;
                if (_speedVec.y > 0) _speedVec.y = 0;
            }
            else
            {
                _isPaintMode.Value = false;
            }
        }

        private void InputHang()
        {
            if (_isAir.Value) return;
            if (_isHang.Value)
            {
                //ハコを持っている際の処理
                var tuple = CheckCanPutBox.Invoke(_bodyDirection.Value);
                if (tuple.Item1)
                {
                    if (Input.GetButtonDown("Decide"))
                    {
                        //ハコを置く
                        _isHang.Value = false;
                        Vector2Int putPos = tuple.Item2;
                        _putBoxSubject.OnNext(new ChipData(ChipEnum.Box, null, putPos.x, putPos.y));
                        UniTask.Void(async () =>
                        {
                            _canOperate = false;
                            PlayPaintAction?.Invoke();
                            await UniTask.Delay(300);
                            _canOperate = true;
                        });
                    }
                }
            }
            else
            {
                //ハコを持っていない際の処理
                var box = CheckBoxAction?.Invoke(_bodyDirection.Value);
                if (box != null)
                {
                    if (Input.GetButtonDown("Decide"))
                    {
                        //ハコを持つ
                        _isHang.Value = true;
                        _removeBoxSubject.OnNext(box.ChipData);
                    }
                }
            }
        }

        /// <summary>
        /// 次の足場が空中かどうかをチェックする
        /// </summary>
        private void CheckNextStepIsAir(bool isBeforeAir)
        {
            if (isBeforeAir) return;
            if (_bodyDirection.Value.Equals(Direction.Down)) return;
            if (CheckIsAirAction.Invoke(_bodyDirection.Value))
            {
                if (_bodyDirection.Value is Direction.Down or Direction.Up)
                {
                    _pos.Value -= Vector3.right * (_speedVec.x * Time.deltaTime);
                }
                else
                {
                    _pos.Value -= Vector3.up * (_speedVec.y * Time.deltaTime);
                }
            }
        }

        /// <summary>
        /// ペイントモードの入力を処理する
        /// </summary>
        private void InputPaint()
        {
            if (_nowSelectedChip != null) _nowSelectedChip.SetPaintSignActive(false);

            if (!_isPaintMode.Value) return;

            //WASDキーを検知する
            var x = Input.GetAxisRaw("Horizontal");
            var y = Input.GetAxisRaw("Vertical");
            var direction = Direction.None;
            switch (x)
            {
                case > 0:
                    //右
                    direction = Direction.Right;
                    break;
                case < 0:
                    //左
                    direction = Direction.Left;
                    break;
            }

            switch (y)
            {
                case > 0:
                    //上
                    direction = Direction.Up;
                    break;
                case < 0:
                    //下
                    direction = Direction.Down;
                    break;
            }

            if (direction == Direction.None) return;

            BlockChip chip = CheckCanPaintAction?.Invoke(_bodyDirection.Value, direction);
            if (chip != null)
            {
                chip.SetPaintSignActive(true);
                _nowSelectedChip = chip;

                //Directionキーが押されたことを検知して、ペイントする
                if (Input.GetButtonUp("Paint"))
                {
                    chip.Paint(CalcUtils.ReverseDirection(direction));
                    _isPainted = true;
                    _paintCount++;

                    UniTask.Void(async () =>
                    {
                        chip.SetPaintSignActive(false);
                        _canOperate = false;
                        PlayPaintAction?.Invoke();
                        await UniTask.Delay(500);
                        _canOperate = true;
                    });
                }
            }
        }

        /// <summary>
        /// 速度情報をView側に送信する
        /// </summary>
        private void SpeedUpdate()
        {
            switch (_bodyDirection.Value)
            {
                case Direction.Down:
                    _onUpdateSpeedSubject.OnNext(_speedVec);
                    break;
                case Direction.Up:
                    _onUpdateSpeedSubject.OnNext(-_speedVec);
                    break;
                case Direction.Left:
                    _onUpdateSpeedSubject.OnNext(new Vector3(-_speedVec.y, _speedVec.x, 0));
                    break;
                case Direction.Right:
                    _onUpdateSpeedSubject.OnNext(new Vector3(_speedVec.y, _speedVec.x, 0));
                    break;
            }
        }

        /// <summary>
        /// 重力ペンキによる体の向きの変更処理
        /// </summary>
        private void Rotate()
        {
            if (_isAir.Value)
            {
                _bodyDirection.Value = Direction.Down;
            }

            if (_isRightCollide && _speedVec.x > 0)
            {
                _bodyDirection.Value = Direction.Right;
            }

            if (_isLeftCollide && _speedVec.x < 0)
            {
                _bodyDirection.Value = Direction.Left;
            }

            if (_isUpCollide && _speedVec.y > 0)
            {
                _bodyDirection.Value = Direction.Up;
            }

            if (_isDownCollide && _speedVec.y < 0)
            {
                _bodyDirection.Value = Direction.Down;
            }
        }

        /// <summary>
        /// 周囲のオブジェクトとめりこみを解消する
        /// </summary>
        private void ResolveCollision()
        {
            //衝突フラグの初期化
            _isDownCollide = false;
            _isUpCollide = false;
            _isLeftCollide = false;
            _isRightCollide = false;

            Vector3 colliderSize;
            Vector3 colliderOffset;
            var size = _myCollider.size;
            var offset = _myCollider.offset;
            switch (_bodyDirection.Value)
            {
                case Direction.Down:
                    colliderSize = _myCollider.size;
                    colliderOffset = _myCollider.offset;
                    break;
                case Direction.Up:
                    colliderSize = _myCollider.size;
                    colliderOffset = -_myCollider.offset;
                    break;
                case Direction.Right:
                    colliderSize = new Vector3(size.y, size.x, 0);
                    colliderOffset = new Vector3(-offset.y, offset.x, 0);
                    break;
                case Direction.Left:
                    colliderSize = new Vector3(size.y, size.x, 0);
                    colliderOffset = new Vector3(offset.y, -offset.x, 0);
                    break;
                default:
                    throw new Exception("BodyDirectionの値が不正です。");
            }

            var newPos = _pos.Value + colliderOffset;
            foreach (BoxCollider2D collider in _nearChipColliders)
            {
                var otherPos = collider.gameObject.transform.position;
                var otherSize = collider.size;
                var chip = collider.gameObject.GetComponent<BlockChip>();

                //垂直方向のめり込み解消
                if ((newPos.x + colliderSize.x / 2) - (otherPos.x - otherSize.x / 2f) > _nigligibleError
                    && (newPos.x - colliderSize.x / 2) - (otherPos.x + otherSize.x / 2f) < -_nigligibleError)
                {
                    if (otherPos.y + otherSize.y / 2 > newPos.y - colliderSize.y / 2)
                    {
                        //上方向に戻す
                        if (otherPos.y + otherSize.y / 2 - _beforePos.y - colliderSize.y / 2 < 0)
                        {
                            if (chip.CanStick(Direction.Up))
                            {
                                _isDownCollide = true;
                            }

                            newPos.y = otherPos.y + otherSize.y / 2f + colliderSize.y / 2f;
                        }
                    }

                    if (otherPos.y - otherSize.y / 2 < newPos.y + colliderSize.y / 2)
                    {
                        //下方向に戻す
                        if (otherPos.y - otherSize.y / 2 - _beforePos.y + colliderSize.y / 2 > 0)
                        {
                            if (chip.CanStick(Direction.Down))
                            {
                                _isUpCollide = true;
                            }

                            newPos.y = otherPos.y - otherSize.y / 2f - colliderSize.y / 2f;
                        }
                    }
                }

                //水平方向のめり込み解消
                if ((newPos.y + colliderSize.y / 2) - (otherPos.y - otherSize.y / 2f) > _nigligibleError
                    && (newPos.y - colliderSize.y / 2) - (otherPos.y + otherSize.y / 2f) < -_nigligibleError)
                {
                    if (otherPos.x + otherSize.x / 2 > newPos.x - colliderSize.x / 2)
                    {
                        //右方向に戻す
                        if (otherPos.x + otherSize.x / 2f - _beforePos.x - colliderSize.x / 2 < 0)
                        {
                            if (chip.CanStick(Direction.Right))
                            {
                                _isLeftCollide = true;
                            }

                            newPos.x = otherPos.x + otherSize.x / 2f + colliderSize.x / 2f;
                        }
                    }

                    if (otherPos.x - otherSize.x / 2f < newPos.x + colliderSize.x / 2f)
                    {
                        //左方向に戻す
                        if (otherPos.x - otherSize.x / 2f - _beforePos.x + colliderSize.x / 2f > 0)
                        {
                            if (chip.CanStick(Direction.Left))
                            {
                                _isRightCollide = true;
                            }

                            newPos.x = otherPos.x - otherSize.x / 2f - colliderSize.x / 2f;
                        }
                    }
                }
            }

            _beforePos = _pos.Value + colliderOffset;
            _pos.Value = newPos - colliderOffset;
        }

        /// <summary>
        /// 重力を適用する
        /// </summary>
        private void ApplyGravity()
        {
            if (_isAir.Value)
            {
                _speedVec.y -= GravityScale * Time.deltaTime;
            }
            else if (NowBodyDirection.Value is Direction.Down or Direction.Up)
            {
                _speedVec.y = 0;
            }
        }

        /// <summary>
        /// ユーザーの入力(移動)を処理する
        /// </summary>
        private void InputMove()
        {
            if (_isPainted) return;
            if (!_canOperate)
            {
                _speedVec = Vector3.zero;
                return;
            }

            Vector3 newSpeedVec = _speedVec;
            if (NowBodyDirection.Value is Direction.Down or Direction.Up)
            {
                var x = Input.GetAxis("Horizontal");
                newSpeedVec.x = x switch
                {
                    > 0 => MoveSpeed,
                    < 0 => -MoveSpeed,
                    _ => _isStickJump ? _speedVec.x : 0
                };
            }
            else
            {
                var y = Input.GetAxis("Vertical");
                newSpeedVec.y = y switch
                {
                    > 0 => MoveSpeed,
                    < 0 => -MoveSpeed,
                    _ => _isStickJump ? _speedVec.y : 0
                };
            }

            _speedVec = newSpeedVec;
        }

        /// <summary>
        /// プレイヤーの入力(ジャンプ)を処理する
        /// </summary>
        private void InputJump()
        {
            if (!_isAir.Value && Input.GetButton("Jump"))
            {
                switch (_bodyDirection.Value)
                {
                    case Direction.Down:
                        _speedVec.y = JumpPower;
                        break;
                    case Direction.Up:
                        _speedVec.y = -JumpPower;
                        break;
                    case Direction.Right:
                        _speedVec.x = -JumpPower;
                        break;
                    case Direction.Left:
                        _speedVec.x = JumpPower;
                        break;
                }

                _isAir.Value = true;
                if (!_bodyDirection.Value.Equals(Direction.Down))
                {
                    _isStickJump = true;
                }
            }
        }

        public void AddNearChipCollider(BoxCollider2D collider)
        {
            _nearChipColliders.Add(collider);
        }

        public void RemoveNearChipCollider(BoxCollider2D collider)
        {
            _nearChipColliders.Remove(collider);
        }

        /// <summary>
        /// 位置を変更させる
        /// </summary>
        public void OnPosChanged(Vector3 newPos)
        {
            _pos.Value = newPos;
        }
    }
}