using Sabanishi.MainGame.Stage;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Sabanishi.MainGame
{
    /// <summary>
    /// ゲームロジックのトップモジュール
    /// </summary>
    public class GameLogicMaster:MonoBehaviour
    {
        [SerializeField] private PlayerPresenter _playerPresenterPrefab;
        [SerializeField] private StagePresenter _stagePresenter;

        [SerializeField] private Tilemap _tmpTileMap;
        private PlayerPresenter _player;

        /// <summary>
        /// Screen生成時に呼ばれる処理
        /// </summary>
        public void Initialize()
        {
            _player = Instantiate(_playerPresenterPrefab,transform);
            _stagePresenter.Initialize(_tmpTileMap);
            _player.Initialize();
        }

        public void Dispose()
        {
            _player.Dispose();
            _stagePresenter.Dispose();
        }
    }
}