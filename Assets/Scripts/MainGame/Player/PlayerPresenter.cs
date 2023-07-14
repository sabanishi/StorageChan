using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Sabanishi.MainGame
{
    public class PlayerPresenter:MonoBehaviour
    {
        [SerializeField] private PlayerView _view;
        private PlayerModel _model;

        /// <summary>
        /// 初期化関数
        /// </summary>
        public void Initialize(Vector3 startPos)
        {
            _model = new PlayerModel();
            _view.Initialize();
            _model.Initialize(startPos);
            
            //Modelとの紐付け
            _model.Pos.Subscribe(_view.OnPosChanged).AddTo(gameObject);
            _model.IsAir.Subscribe(_view.OnIsAirChanged).AddTo(gameObject);
            _model.IsHang.Subscribe(_view.OnIsHangChanged).AddTo(gameObject);
            _model.NowBodyDirection.Subscribe(_view.OnBodyDirectionChanged).AddTo(gameObject);
        }

        /// <summary>
        /// 破壊時処理
        /// </summary>
        public void Dispose()
        {
            _model.Dispose();
        }

        public void Update()
        {
            _model.Update();
        }
        
        
    }
}