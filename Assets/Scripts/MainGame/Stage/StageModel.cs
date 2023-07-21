using UniRx;

namespace Sabanishi.MainGame.Stage
{
    public class StageModel
    {
        private int _stageWidth;
        private int _stageHeight;
        private readonly ReactiveCollection<ChipData> _stageData;
        public IReadOnlyReactiveCollection<ChipData> StageData => _stageData;

        public StageModel(int width,int height)
        {
            _stageWidth = width;
            _stageHeight = height;
            _stageData = new ();
        }

        public void Dispose()
        {
            _stageData.Dispose();
        }

        /// <summary>
        /// chipDataを基にステージを作成する
        /// </summary>
        public void CreateBlock(ChipData[,] chipData)
        {
            for (int y = 0; y < chipData.GetLength(1); y++)
            {
                for(int x = 0; x < chipData.GetLength(0); x++)
                {
                    _stageData.Add(chipData[x, y]);
                }
            }
        }
    }
}