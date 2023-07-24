using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using Sabanihsi.ScreenSystem;
using Sabanishi.MainGame.Stage;
using UniRx;
using Unity.VisualScripting;
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
        [SerializeField] private MainCamera _mainCamera;
        [SerializeField]private StartEffectAnimation _startEffectAnimation;
        
        private PlayerPresenter _player;
        private int _doorX;
        private Action _gotoStageSelectAction;
        
        private Dictionary<string, string> _stageTilemapPathDict = new()
        {
            { "Tmp", "Tilemap/TmpTilemap" },
        };

        /// <summary>
        /// Screen生成時に呼ばれる処理
        /// </summary>
        public void Initialize(StageData stageData,Action gotoStageSelectAction)
        {
            _gotoStageSelectAction = gotoStageSelectAction;
            _player = Instantiate(_playerPresenterPrefab,transform);

            if (TryGetStageTilemap(stageData.StageName, out var tilemap))
            {
                _stagePresenter.Initialize(tilemap);
            }
            
            _player.Initialize(_stagePresenter.PlayerRespawnPos);
            _player.AddBoxSubject.Subscribe(_stagePresenter.Model.AddBox).AddTo(gameObject);
            _player.RemoveBoxSubject.Subscribe(_stagePresenter.Model.RemoveBox).AddTo(gameObject);
            
            _mainCamera.Initialize(_player.transform,_stagePresenter.MapSize);

            _doorX = _stagePresenter.DoorPos.x;
        }

        public void Open()
        {
            _player.Model.SetCanOperate(true);
            _startEffectAnimation.StartEffect(_stagePresenter.PlayerRespawnPos);
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

        private void Update()
        {
            if (Input.GetButtonDown("Reset"))
            {
                ScreenTransition.Instance.Move(ScreenEnum.MainGame).Forget();
            }
            var isClear = CheckIsClear();
            if (isClear)
            {
                //TODO:クリア処理
                _player.Model.SetCanOperate(false);
                _gotoStageSelectAction?.Invoke();
                async UniTaskVoid Clear()
                {
                    await UniTask.Delay(500);
                    ScreenTransition.Instance.Move(ScreenEnum.Home).Forget();
                }
                Clear().Forget();
            }
        }

        /// <summary>
        /// クリアフラグをチェックする
        /// </summary>
        private bool CheckIsClear()
        {
            if (_player.Model.IsHang.Value) return false;

            return _stagePresenter.IsAllBoxIndoor(_doorX);
        }
    }
}