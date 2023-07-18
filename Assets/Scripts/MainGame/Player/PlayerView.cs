using System;
using UniRx;
using UnityEngine;

namespace Sabanishi.MainGame
{
    public class PlayerView:MonoBehaviour
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
            _rayLength = size.y/2;
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
            _animator.SetBool(IsAir,isAir);
        }
        
        public void OnIsHangChanged(bool isHang)
        {
            _animator.SetBool(IsHang,isHang);
        }

        public void OnSpeedChanged(Vector3 vec)
        {
            _animator.SetFloat(YSpeed,vec.y);
            _animator.SetBool(IsWalk,vec.x!=0);
            if (vec.x > 0)
            {
                _transform.localScale=new Vector3(1,1,1);
            }else if (vec.x < 0)
            {
                _transform.localScale=new Vector3(-1,1,1);
            }
        }
        
        public void OnBodyDirectionChanged(BodyDirection bodyDirection)
        {
            float diff = (_colliderOffset.y+_colliderOffset.x)/2;
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
                    _posChangeSubject.OnNext(_transform.position+Vector3.left*(_colliderSize.y-_colliderSize.x)/2+new Vector3(diff,diff,0));
                    break;
                case BodyDirection.Left:
                    _transform.localEulerAngles = new Vector3(0,0,-90);
                    _posChangeSubject.OnNext(_transform.position+Vector3.right*(_colliderSize.y-_colliderSize.x)/2+new Vector3(-diff,diff,0));
                    break;
            }
        }
        
        /// <summary>
        /// 足元をチェックして空中にいるかどうかを返す
        /// </summary>
        /// <returns></returns>
        public bool CheckIsAir(BodyDirection bodyDirection)
        {
            //足元に何もない場合、空中にいると判定する
            Vector2 direction;
            Vector2 offset;
            switch (bodyDirection)
            {
                case BodyDirection.Down:
                    direction=Vector2.down;
                    offset = _colliderOffset;
                    break;
                case BodyDirection.Up: 
                    direction=Vector2.up;
                    offset = -_colliderOffset;
                    break;
                case BodyDirection.Right:
                    direction=Vector2.right;
                    offset = new Vector2(-_colliderOffset.y,_colliderOffset.x);
                    break;
                case BodyDirection.Left: 
                    direction=Vector2.left; 
                    offset = new Vector2(_colliderOffset.y,-_colliderOffset.x);
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(bodyDirection), bodyDirection, null);
            }
            return Physics2D.Raycast((Vector2)_transform.position+offset, direction, _rayLength, LayerMask.GetMask("Block")) == false;
        }
    }
}