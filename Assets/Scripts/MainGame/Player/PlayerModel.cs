using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Sabanishi.MainGame
{
    public class PlayerModel
    {
        private const float MoveSpeed = 5.0f;
        private const float GravityScale = 9.8f;
        private const float JumpPower = 5.0f;
        private const float _nigligibleError = 0.2f;
        
        #region ReactiveProperty

        private ReactiveProperty<bool> _isAir;
        public IReadOnlyReactiveProperty<bool> IsAir => _isAir;

        private Vector3 _beforePos;//1フレーム前の座標
        private ReactiveProperty<Vector3> _pos;
        public IReadOnlyReactiveProperty<Vector3> Pos => _pos;

        private ReactiveProperty<bool> _isHang;
        public IReadOnlyReactiveProperty<bool> IsHang => _isHang;
        
        private ReactiveProperty<BodyDirection> _bodyDirection;
        public IReadOnlyReactiveProperty<BodyDirection> NowBodyDirection => _bodyDirection;

        #endregion

        private Vector3 _speedVec;
        private BoxCollider2D _myCollider;
        private List<BoxCollider2D> _nearChipColliders;
        private Vector3 _colliderSize;
        private Vector3 _colliderOffset;

        public PlayerModel()
        {
            _isAir = new();
            _pos = new();
            _isHang = new();
            _bodyDirection = new();
            _speedVec=Vector3.zero;
            _nearChipColliders = new();
        }

        public void Initialize(Vector3 startPos,BoxCollider2D myCollider)
        {
            _pos.Value = startPos;
            _isAir.Value = false;
            _isHang.Value = false;
            _bodyDirection.Value = BodyDirection.Down;
            _myCollider = myCollider;
            _colliderSize = _myCollider.size;
            _colliderOffset = _myCollider.offset;

            _beforePos = startPos+_colliderOffset;
        }

        public void Dispose()
        {
            _isAir.Dispose();
            _pos.Dispose();
            _isHang.Dispose();
            _bodyDirection.Dispose();
        }

        /// <summary>
        /// 毎フレーム実行される処理
        /// </summary>
        public void Update(Vector3 beforePos,bool isAir)
        {
            _pos.Value = beforePos;
            _isAir.Value = isAir;
            InputMove();
            ApplyGravity();

            //移動処理
            _pos.Value += _speedVec * Time.deltaTime;

            ResolveCollision();
        }

        /// <summary>
        /// 周囲のオブジェクトとめりこみを解消する
        /// </summary>
        private void ResolveCollision()
        {
            var newPos = _pos.Value+_colliderOffset;
            foreach (BoxCollider2D collider in _nearChipColliders)
            {
                var otherPos = collider.gameObject.transform.position;
                var otherSize = collider.size;

                //垂直方向のめり込み解消
                if ((newPos.x + _colliderSize.x/2) - (otherPos.x - otherSize.x / 2f) >_nigligibleError
                    && (newPos.x - _colliderSize.x/2) - (otherPos.x + otherSize.x / 2f)<-_nigligibleError)
                {
                    Debug.Log("(newPos.x + _colliderSize.x/2) - (otherPos.x - otherSize.x / 2f): "+((newPos.x + _colliderSize.x/2) - (otherPos.x - otherSize.x / 2f)));
                    Debug.Log("(newPos.x - _colliderSize.x/2) - (otherPos.x + otherSize.x / 2f): "+((newPos.x - _colliderSize.x/2) - (otherPos.x + otherSize.x / 2f)));
                    if (otherPos.y+otherSize.y/2 > newPos.y-_colliderSize.y/2)
                    {
                        //上方向
                        if (otherPos.y + otherSize.y / 2 - _beforePos.y-_colliderSize.y/2<0)
                        {
                            Debug.Log("上方向");
                            newPos.y = otherPos.y + otherSize.y/2f + _colliderSize.y/2f;
                        }
                    }
                    else if (otherPos.y-otherSize.y/2 < newPos.y+_colliderSize.y/2)
                    {
                        //下方向
                        if (otherPos.y - otherSize.y / 2 - _beforePos.y+_colliderSize.y/2 > 0)
                        {
                            Debug.Log("下方向");
                            newPos.y = otherPos.y - otherSize.y/2f - _colliderSize.y/2f;
                        }
                       
                    }
                }
                
                //水平方向のめり込み解消
                if ((newPos.y + _colliderSize.y/2) -(otherPos.y - otherSize.y / 2f)>_nigligibleError
                    && (newPos.y - _colliderSize.y/2) - (otherPos.y + otherSize.y / 2f)<-_nigligibleError)
                {
                    if (otherPos.x+otherSize.x/2 > newPos.x-_colliderSize.x/2)
                    {
                        //右方向
                        if (otherPos.x + otherSize.x / 2 - _beforePos.x-_colliderSize.x/2<0)
                        {
                            Debug.Log("右方向");
                            newPos.x = otherPos.x + otherSize.x/2 + _colliderSize.x/2;
                        }
                    }
                    else if (otherPos.x-otherSize.x/2 < newPos.x+_colliderSize.x/2)
                    {
                        //左方向
                        if (otherPos.x - otherSize.x / 2 - _beforePos.x+_colliderSize.x/2 > 0)
                        {
                            Debug.Log("左方向");
                            newPos.x = otherPos.x - otherSize.x/2f - _colliderSize.x/2f;
                        }
                       
                    }
                }
            }
            _beforePos = _pos.Value+_colliderOffset;
            _pos.Value = newPos-_colliderOffset;
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
            else
            {
                _speedVec.y = 0;
            }
        }

        /// <summary>
        /// ユーザーの入力を処理する
        /// </summary>
        private void InputMove()
        {
            Vector3 newSpeedVec = _speedVec;
            //歩行処理
            if (NowBodyDirection.Value is BodyDirection.Down or BodyDirection.Up)
            {
                var x = Input.GetAxis("Horizontal");
                newSpeedVec.x = x switch
                {
                    > 0 => MoveSpeed,
                    < 0 => -MoveSpeed,
                    _ => 0
                };
            }
            else
            {
                var y = Input.GetAxis("Vertical");
                newSpeedVec.y = y switch
                {
                    > 0 => MoveSpeed,
                    < 0 => -MoveSpeed,
                    _ => 0
                };
            }
            
            //ジャンプ処理
            if (!_isAir.Value&&Input.GetButtonDown("Jump"))
            {
                newSpeedVec.y = JumpPower;
                _isAir.Value = true;
            }

            _speedVec= newSpeedVec;
        }
        
        public void AddNearChipCollider(BoxCollider2D collider)
        {
            _nearChipColliders.Add(collider);
        }
        
        public void RemoveNearChipCollider(BoxCollider2D collider)
        {
            _nearChipColliders.Remove(collider);
        }
    }
}