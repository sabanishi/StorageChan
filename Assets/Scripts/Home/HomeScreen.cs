using System.Threading;
using Cysharp.Threading.Tasks;
using Sabanihsi.ScreenSystem;

namespace Sabanishi.Home
{
    public class HomeScreen:AbstractScreen
    {
        private StageData _stageData;

        public void SetStageData(StageData data)
        {
            _stageData = data;
        }

        protected override async UniTask<StageData> CloseInternal(CancellationToken token)
        {
            return _stageData;
        }
    }
}