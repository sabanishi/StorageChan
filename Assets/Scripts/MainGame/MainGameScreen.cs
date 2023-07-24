using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sabanihsi.ScreenSystem;
using UnityEngine;

namespace Sabanishi.MainGame
{
    public class MainGameScreen:AbstractScreen
    {
        [SerializeField] private GameLogicMaster _gameLogic;
        

        protected override void InitializeInternal(StageData data,CancellationToken token)
        {
            _stageData = data;
            _gameLogic.Initialize(data,PrepareGoToStageSelectAction);
        }
        
        protected override UniTask OpenInternal(CancellationToken token)
        {
            _gameLogic.Open();
            return UniTask.CompletedTask;
        }
        
        protected override void DisposeInternal()
        {
            _gameLogic.Dispose();
        }
        
        protected override  async UniTask<StageData> CloseInternal(CancellationToken token)
        {
            return _stageData;
        }
        
        private void PrepareGoToStageSelectAction()
        {
            _stageData = new StageData("ステージ セレクト");
        }
    }
}