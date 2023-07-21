using Sabanihsi.ScreenSystem;
using Object = System.Object;

namespace Sabanishi.Home
{
    public class HomeScreen:AbstractScreen
    {
        private StageData _stageData;

        public void SetStageData(StageData data)
        {
            _stageData = data;
        }

        protected override Object DisposeInternal()
        {
            return _stageData;
        }
    }
}