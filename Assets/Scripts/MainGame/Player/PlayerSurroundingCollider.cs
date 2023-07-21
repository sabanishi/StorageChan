using System;
using UniRx;
using UnityEngine;

namespace Sabanishi.MainGame
{
    /// <summary>
    /// プレイヤーの周囲にあるChipを検知するためのCollider
    /// </summary>
    public class PlayerSurroundingCollider:MonoBehaviour
    {
        private const string BlockTag = "Block";
        private const string BoxTag = "Box";
        private Subject<BoxCollider2D> _addSubject;
        public IObservable<BoxCollider2D> OnAdd => _addSubject;
        private Subject<BoxCollider2D> _removeSubject;
        public IObservable<BoxCollider2D> OnRemove => _removeSubject;

        public void Initialize()
        {
            _addSubject = new();
            _removeSubject= new();
        }

        public void Dispose()
        {
            _addSubject.Dispose();
            _removeSubject.Dispose();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag.Equals(BlockTag)||other.gameObject.tag.Equals(BoxTag))
            {
                _addSubject?.OnNext(other as BoxCollider2D);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.tag.Equals(BlockTag)||other.gameObject.tag.Equals(BoxTag))
            {
                _removeSubject?.OnNext(other as BoxCollider2D);
            }
        }
    }
}