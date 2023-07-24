using UnityEngine;

namespace Sabanihsi.ScreenSystem
{
    public class PlayerImage:MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private GameObject _box;
        
        private static readonly int IsHang = Animator.StringToHash("IsHang");
        private static readonly int IsWalk = Animator.StringToHash("IsWalk");
        private static readonly int PaintWalk = Animator.StringToHash("paintWalk");
        private static readonly int PaintWalkStop = Animator.StringToHash("paintWalkStop");
        
        public void StartBoxWalk()
        {
            _animator.SetTrigger(PaintWalkStop);
            _animator.SetBool(IsHang,true);
            _animator.SetBool(IsWalk,true);
            _box.SetActive(true);
        }

        public void StartPaintWalk()
        {
            _box.SetActive(false);
            _animator.SetBool(IsHang,false);
            _animator.SetTrigger(PaintWalk);
        }
    }
}