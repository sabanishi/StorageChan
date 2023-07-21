namespace Sabanihsi.ScreenSystem
{
    public class StageData
    {
        private string _stageName;
        public string StageName => _stageName;
        
        public StageData(string stageName)
        {
            _stageName = stageName;
        }
    }
}