namespace Sabanihsi.ScreenSystem
{
    public class StageData
    {
        private string _stageName;
        public string StageName => _stageName;
        
        private string _tileName;
        public string TileName => _tileName;

        public StageData(string tileName)
        {
            _tileName = tileName;
            SetTitleName();
        }

        private void SetTitleName()
        {
            switch (_tileName)
            {
                case "StageSelect":
                    _stageName = "ステージ セレクト";
                    break;
                case "Stage1":
                    _stageName = "ステージ1";
                    break;
                case "Stage2":
                    _stageName = "ステージ2";
                    break;
                case "Stage3":
                    _stageName = "ステージ3";
                    break;
                case "Stage4":
                    _stageName = "ステージ4";
                    break;
            }
        }
    }
}