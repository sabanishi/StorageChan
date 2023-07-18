using System;
using System.Collections.Generic;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

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

        private Vector3 _beforePos;//1フレーム前の座標
        private readonly ReactiveProperty<Vector3> _pos;
        public IReadOnlyReactiveProperty<Vector3> Pos => _pos;
        private Vector3 _speedVec;

        private readonly ReactiveProperty<bool> _isHang;
        public IReadOnlyReactiveProperty<bool> IsHang => _isHang;
        
        private ReactiveProperty<BodyDirection> _bodyDirection;
        public IReadOnlyReactiveProperty<BodyDirection> NowBodyDirection => _bodyDirection;
        
        private Subject<Vector3> _onUpdateSpeedSubject;
        public IObservable<Vector3> OnUpdateSpeedSubject => _onUpdateSpeedSubject;

        #endregion
        
        private BoxCollider2D _myCollider;
        private List<BoxCollider2D> _nearChipColliders;
        private bool _isRightCollide,_isLeftCollide,_isUpCollide,_isDownCollide;
        private bool _isStickJump;//地上以外からジャンプしたかどうか

        public PlayerModel()
        {
            _isAir = new();
            _pos = new();
            _speedVec = new();
            _isHang = new();
            _bodyDirection = new();
            _nearChipColliders = new();
            _onUpdateSpeedSubject = new();
        }

        public void Initialize(Vector3 startPos,BoxCollider2D myCollider)
        {
            _pos.Value = startPos;
            _beforePos = startPos;
            _isAir.Value = false;
            _isHang.Value = false;
            _bodyDirection.Value = BodyDirection.Down;
            _myCollider = myCollider;
            _speedVec=Vector3.zero;
        }

        public void Dispose()
        {
            _isAir.Dispose();
            _pos.Dispose();
            _isHang.Dispose();
            _bodyDirection.Dispose();
            _onUpdateSpeedSubject.Dispose();
        }

        /// <summary>
        /// 毎フレーム実行される処理
        /// </summary>
        public void Update(Vector3 beforePos,bool isAir)
        {
            //1フレームの間隔が大きすぎる時、処理を飛ばす
            if(Time.deltaTime>0.2f) return;

            if (!isAir)
            {
                _isStickJump = false;
            }

            _pos.Value = beforePos;
            _isAir.Value = isAir;
            InputMove();
            ApplyGravity();
            InputJump();

            //移動処理
            _pos.Value += _speedVec * Time.deltaTime;

            ResolveCollision();
            SpeedUpdate();
            Rotate();

        }
        
        /// <summary>
        /// 速度情報をView側に送信する
        /// </summary>
        private void SpeedUpdate()
        {
            switch (_bodyDirection.Value)
            {
                case BodyDirection.Down:
                    _onUpdateSpeedSubject.OnNext(_speedVec);
                    break;
                case BodyDirection.Up:
                    _onUpdateSpeedSubject.OnNext(-_speedVec);
                    break;
                case BodyDirection.Left:
                    _onUpdateSpeedSubject.OnNext(new Vector3(-_speedVec.y,_speedVec.x,0));
                    break;
                case BodyDirection.Right:
                    _onUpdateSpeedSubject.OnNext(new Vector3(_speedVec.y,_speedVec.x,0));
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
                _bodyDirection.Value = BodyDirection.Down;
            }
            
            if (_isRightCollide && _speedVec.x > 0)
            {
                _bodyDirection.Value = BodyDirection.Right;
            }
            if (_isLeftCollide && _speedVec.x < 0)
            {
                _bodyDirection.Value = BodyDirection.Left;
            }
            
            if (_isUpCollide && _speedVec.y > 0)
            {
                _bodyDirection.Value = BodyDirection.Up;
            }
            if (_isDownCollide && _speedVec.y < 0)
            {
                _bodyDirection.Value = BodyDirection.Down;
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
                case BodyDirection.Down:
                    colliderSize = _myCollider.size;
                    colliderOffset = _myCollider.offset;
                    break;
                case BodyDirection.Up:
                    colliderSize = _myCollider.size;
                    colliderOffset = -_myCollider.offset;
                    break;
                case BodyDirection.Right:
                    colliderSize = new Vector3(size.y,size.x,0);
                    colliderOffset = new Vector3(-offset.y,offset.x,0);
                    break;
                case BodyDirection.Left:
                    colliderSize = new Vector3(size.y,size.x,0);
                    colliderOffset = new Vector3(offset.y,-offset.x,0);
                    break;
                default:
                    throw new Exception("BodyDirectionの値が不正です。");
            }
            var newPos = _pos.Value+colliderOffset;
            foreach (BoxCollider2D collider in _nearChipColliders)
            {
                var otherPos = collider.gameObject.transform.position;
                var otherSize = collider.size;

                //垂直方向のめり込み解消
                if ((newPos.x + colliderSize.x/2) - (otherPos.x - otherSize.x / 2f) >_nigligibleError
                    && (newPos.x - colliderSize.x/2) - (otherPos.x + otherSize.x / 2f)<-_nigligibleError)
                {
                    if (otherPos.y+otherSize.y/2 > newPos.y-colliderSize.y/2)
                    {
                        //上方向に戻す
                        if (otherPos.y + otherSize.y / 2 - _beforePos.y-colliderSize.y/2<0)
                        {
                            _isDownCollide = true;
                            newPos.y = otherPos.y + otherSize.y/2f + colliderSize.y/2f;
                        }
                    }
                    if (otherPos.y-otherSize.y/2 < newPos.y+colliderSize.y/2)
                    {
                        //下方向に戻す
                        if (otherPos.y - otherSize.y / 2 - _beforePos.y+colliderSize.y/2 > 0)
                        {
                            _isUpCollide = true;
                            newPos.y = otherPos.y - otherSize.y/2f - colliderSize.y/2f;
                        }
                       
                    }
                }
                
                //水平方向のめり込み解消
                if ((newPos.y + colliderSize.y/2) -(otherPos.y - otherSize.y / 2f)>_nigligibleError
                    && (newPos.y - colliderSize.y/2) - (otherPos.y + otherSize.y / 2f)<-_nigligibleError)
                {
                    if (otherPos.x+otherSize.x/2 > newPos.x-colliderSize.x/2)
                    {
                        //右方向に戻す
                        if (otherPos.x + otherSize.x / 2f - _beforePos.x-colliderSize.x/2<0)
                        {
                            _isLeftCollide = true;
                            newPos.x = otherPos.x + otherSize.x/2f + colliderSize.x/2f;
                        }
                    }
                    if (otherPos.x-otherSize.x/2f < newPos.x+colliderSize.x/2f)
                    {
                        //左方向に戻す
                        if (otherPos.x - otherSize.x / 2f - _beforePos.x+colliderSize.x/2f > 0)
                        {
                            _isRightCollide = true;
                            newPos.x = otherPos.x - otherSize.x/2f - colliderSize.x/2f;
                        }
                       
                    }
                }
            }
            _beforePos = _pos.Value+colliderOffset;
            _pos.Value = newPos-colliderOffset;
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
            else if(NowBodyDirection.Value is BodyDirection.Down or BodyDirection.Up)
            {
                _speedVec.y = 0;
            }
        }

        /// <summary>
        /// ユーザーの入力(移動)を処理する
        /// </summary>
        private void InputMove()
        {
            Vector3 newSpeedVec = _speedVec;
            if (NowBodyDirection.Value is BodyDirection.Down or BodyDirection.Up)
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
            _speedVec= newSpeedVec;
        }

        /// <summary>
        /// プレイヤーの入力(ジャンプ)を処理する
        /// </summary>
        private void InputJump()
        {
            if (!_isAir.Value&&Input.GetButtonDown("Jump"))
            {
                switch (_bodyDirection.Value)
                {
                    case BodyDirection.Down:
                        _speedVec.y = JumpPower; break;
                    case BodyDirection.Up: _speedVec.y = -JumpPower; break;
                    case BodyDirection.Right: _speedVec.x = -JumpPower; break;
                    case BodyDirection.Left: _speedVec.x = JumpPower; break;
                }
                _isAir.Value = true;
                if (!_bodyDirection.Value.Equals(BodyDirection.Down))
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