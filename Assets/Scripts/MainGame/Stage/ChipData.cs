using UnityEngine;

namespace Sabanishi.MainGame
{
    public struct ChipData
    {
        public ChipEnum ChipEnum { get; }
        public Sprite Image { get; }
        public int X { get; }
        public int Y { get; }

        public ChipData(ChipEnum chipEnum, Sprite image,int x,int y)
        {
            ChipEnum = chipEnum;
            Image = image;
            X = x;
            Y = y;
        }
    }
}