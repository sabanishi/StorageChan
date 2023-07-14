using UniRx;
using UnityEngine;

namespace Sabanishi.MainGame
{
    public class PlayerModel
    {
        #region ReactiveProperty

        private ReactiveProperty<bool> _isAir;
        public IReadOnlyReactiveProperty<bool> IsAir => _isAir;

        private ReactiveProperty<Vector3> _pos;
        public IReadOnlyReactiveProperty<Vector3> Pos => _pos;

        private ReactiveProperty<bool> _isHang;
        public IReadOnlyReactiveProperty<bool> IsHang => _isHang;
        
        private ReactiveProperty<BodyDirection> _bodyDirection;
        public IReadOnlyReactiveProperty<BodyDirection> NowBodyDirection => _bodyDirection;

        #endregion
        
        private const float _moveSpeed = 5.0f;

        private Vector3 _speedVec;

        public PlayerModel()
        {
            _isAir = new();
            _pos = new();
            _isHang = new();
            _bodyDirection = new();
            _speedVec=Vector3.zero;
        }

        public void Initialize(Vector3 startPos)
        {
            _pos.Value = startPos;
            _isAir.Value = false;
            _isHang.Value = false;
            _bodyDirection.Value = BodyDirection.Down;
        }

        /// <summary>
        /// 毎フレーム実行される処理
        /// </summary>
        public void Update()
        {
            InputMove();
        }

        /// <summary>
        /// ユーザーの入力による移動処理
        /// </summary>
        private void InputMove()
        {
            Vector3 newSpeedVec = _speedVec;
            //歩行処理
            if (NowBodyDirection.Value is BodyDirection.Down or BodyDirection.Up)
            {
                var x = Input.GetAxis("Horizontal");
                Debug.Log(x);
                newSpeedVec.x = x switch
                {
                    > 0 => _moveSpeed,
                    < 0 => -_moveSpeed,
                    _ => 0
                };
            }
            else
            {
                var y = Input.GetAxis("Vertical");
                newSpeedVec.y = y switch
                {
                    > 0 => _moveSpeed,
                    < 0 => -_moveSpeed,
                    _ => 0
                };
            }
            
            _speedVec= newSpeedVec;
            _pos.Value += _speedVec * Time.deltaTime;
        }

        public void Dispose()
        {
            _isAir.Dispose();
            _pos.Dispose();
            _isHang.Dispose();
            _bodyDirection.Dispose();
        }
    }
}