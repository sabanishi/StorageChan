using Sabanishi.MainGame;
using UnityEngine;

namespace Sabanishi.MainGame
{
    public struct ChipData
    {
        private ChipEnum _chipEnum;
        private Sprite _image;
        public ChipEnum ChipEnum => _chipEnum;
        public Sprite Image => _image;

        public ChipData(ChipEnum chipEnum, Sprite image)
        {
            _chipEnum = chipEnum;
            _image = image;
        }
    }
}