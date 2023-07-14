using System.Threading;
using Cysharp.Threading.Tasks;
using Sabanihsi.ScreenSystem;
using UnityEngine;

namespace Sabanishi.MainGame
{
    public class MainGameScreen:AbstractScreen
    {
        [SerializeField] private GameLogicMaster _gameLogic;
        
        protected override void InitializeInternal(Object data,CancellationToken token)
        {
            _gameLogic.Initialize();
        }
        
        protected override Object DisposeInternal()
        {
            _gameLogic.Dispose();
            return null;
        }
        
        protected override async UniTask OpenInternal(CancellationToken token)
        {
            
        }
        
        protected override  async UniTask CloseInternal(CancellationToken token)
        {
            
        }
    }
}