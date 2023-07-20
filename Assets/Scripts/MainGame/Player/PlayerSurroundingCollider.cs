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
        private const string ChipTag = "Block";
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
            if (other.gameObject.tag.Equals(ChipTag))
            {
                _addSubject?.OnNext(other as BoxCollider2D);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.tag.Equals(ChipTag))
            {
                _removeSubject?.OnNext(other as BoxCollider2D);
            }
        }
    }
}