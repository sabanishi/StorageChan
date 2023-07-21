using System.Threading;
using Cysharp.Threading.Tasks;
using Sabanihsi.ScreenSystem;
using UnityEngine;
using Object = System.Object;

namespace Sabanishi.MainGame
{
    public class MainGameScreen:AbstractScreen
    {
        [SerializeField] private GameLogicMaster _gameLogic;
        
        private StageData _stageData;
        
        protected override void InitializeInternal(Object data,CancellationToken token)
        {
            var stageData = (StageData)data;
            _stageData = stageData;
            _gameLogic.Initialize(stageData);
        }
        
        protected override Object DisposeInternal()
        {
            _gameLogic.Dispose();
            return _stageData;
        }
        
        protected override async UniTask OpenInternal(CancellationToken token)
        {
            
        }
        
        protected override  async UniTask CloseInternal(CancellationToken token)
        {
            
        }
    }
}