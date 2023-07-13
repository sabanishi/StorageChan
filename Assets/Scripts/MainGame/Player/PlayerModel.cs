using UniRx;
using UnityEngine;

namespace Sabanishi.MainGame
{
    public class PlayerModel
    {
        private ReactiveProperty<bool> _isAir;
        public IReadOnlyReactiveProperty<bool> IsAir => _isAir;

        private ReactiveProperty<Vector3> _pos;
        public IReadOnlyReactiveProperty<Vector3> Pos => _pos;

        private ReactiveProperty<bool> _isHang;
        public IReadOnlyReactiveProperty<bool> IsHang => _isHang;

        public PlayerModel()
        {
            _isAir = new();
            _pos = new();
            _isHang = new();
        }

        public void Initialize(Vector3 startPos)
        {
            _pos.Value = startPos;
        }

        public void Dispose()
        {
            _isAir.Dispose();
            _pos.Dispose();
            _isHang.Dispose();
        }
    }
}