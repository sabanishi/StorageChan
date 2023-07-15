using UnityEngine;

namespace Sabanishi.MainGame
{
    public class PlayerView:MonoBehaviour
    {
        private Animator _animator;
        private Transform _transform;
        private static readonly int IsAir = Animator.StringToHash("IsAir");
        private static readonly int IsHang = Animator.StringToHash("IsHang");

        public void Initialize()
        {
            _transform = transform;
            _animator = GetComponent<Animator>();
        }

        public void OnPosChanged(Vector3 pos)
        {
            _transform.position = pos;
        }

        public void OnIsAirChanged(bool isAir)
        {
            _animator.SetBool(IsAir,isAir);
        }
        
        public void OnIsHangChanged(bool isHang)
        {
            _animator.SetBool(IsHang,isHang);
        }
        
        public void OnBodyDirectionChanged(BodyDirection bodyDirection)
        {
            switch (bodyDirection)
            {
                case BodyDirection.Down:
                    _transform.localEulerAngles = new Vector3(0,0,0);
                    break;
                case BodyDirection.Up:
                    _transform.localEulerAngles = new Vector3(0,0,180);
                    break;
                case BodyDirection.Right:
                    _transform.localEulerAngles = new Vector3(0,0,90);
                    break;
                case BodyDirection.Left:
                    _transform.localEulerAngles = new Vector3(0,0,-90);
                    break;
            }
        }
        
        /// <summary>
        /// 足元をチェックして空中にいるかどうかを返す
        /// </summary>
        /// <returns></returns>
        public bool CheckIsAir()
        {
            //足元に何もない場合、空中にいると判定する
            return Physics2D.Raycast(_transform.position, Vector2.down, 0.503f, LayerMask.GetMask("Block")) == false;
        }
    }
}