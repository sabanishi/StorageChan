using Sabanishi.MainGame;
using UnityEngine;

namespace MainGame.Stage
{
    public class Box:MonoBehaviour
    {
        private ChipData _chipData;

        public ChipData ChipData => _chipData;

        public void Initialize(ChipData chipData)
        {
            _chipData = chipData;
        }
    }
}