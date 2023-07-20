using System.Collections.Generic;
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
        
        private PlayerPresenter _player;
        
        private Dictionary<string, string> _stageTilemapPathDict = new()
        {
            { "Tmp", "Tilemap/TmpTilemap" },
        };

        /// <summary>
        /// Screen生成時に呼ばれる処理
        /// </summary>
        public void Initialize()
        {
            _player = Instantiate(_playerPresenterPrefab,transform);

            if (TryGetStageTilemap("Tmp", out var tilemap))
            {
                _stagePresenter.Initialize(tilemap);
            }
            
            _player.Initialize(_stagePresenter.GetPlayerRespawnPos());
        }

        public void Dispose()
        {
            _player.Dispose();
            _stagePresenter.Dispose();
        }

        private bool TryGetStageTilemap(string stageName, out Tilemap tilemap)
        {
            tilemap = null;
            if (!_stageTilemapPathDict.TryGetValue(stageName, out var path))
            {
                Debug.LogError("[ScreenTransition] ScreenEnumに対応するパスが見つかりませんでした");
                return false;
            }

            tilemap = Resources.Load<Tilemap>(path);
            if (tilemap == null)
            {
                Debug.Log("[ScreenTransition] Tilemapの取得に失敗しました");
            }

            return true;
        }
    }
}