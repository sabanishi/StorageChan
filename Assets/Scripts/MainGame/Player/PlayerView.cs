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
    }
}