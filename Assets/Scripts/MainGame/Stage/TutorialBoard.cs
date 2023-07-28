using Sabanishi.MainGame;
using UnityEngine;

namespace MainGame.Stage
{
    public class TutorialBoard:MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _explainSprite;

        public void Initialize(ChipData data)
        {
            _explainSprite.sprite = data.Image;
            transform.position += new Vector3(-0.5f, 0.5f, 0);
        }
    }
}