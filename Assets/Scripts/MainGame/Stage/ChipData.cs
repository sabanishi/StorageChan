using UnityEngine;

namespace Sabanishi.MainGame
{
    public class ChipData
    {
        public ChipEnum ChipEnum { get; }
        public Sprite Image { get; }
        public int X { get; }
        public int Y { get; private set; }
        
        public int StageNum { get; private set; }

        public void SetY(int y)
        {
            Y = y;
        }

        public ChipData(ChipEnum chipEnum, Sprite image,int x,int y,int stageNum = -1)
        {
            ChipEnum = chipEnum;
            Image = image;
            X = x;
            Y = y;
            StageNum = stageNum;
        }
    }
}