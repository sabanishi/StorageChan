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
        public void Initialize()
        {
            _model = new PlayerModel();
            _view.Initialize();
        }

        /// <summary>
        /// 破壊時処理
        /// </summary>
        public void Dispose()
        {
            
        }
        
        
    }
}