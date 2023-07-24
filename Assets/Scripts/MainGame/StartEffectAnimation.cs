using DG.Tweening;
using UnityEngine;

namespace Sabanishi.MainGame
{
    public class StartEffectAnimation:MonoBehaviour
    {
        [SerializeField] private Sprite _image1;
        [SerializeField] private Sprite _image2;
        [SerializeField] private Sprite _image3;
        [SerializeField] private Sprite _image4;
        
        public void StartEffect(Vector3 startPos)
        {
            for (var i = 0; i < 5; i++)
            {
                CreateEffect(startPos,i);
            }
        }
        
        private void CreateEffect(Vector3 startPos,int i)
        {
            Sprite sprite = GetSprite(i);
            var obj = new GameObject($"StartEffect({i})");
            obj.transform.parent= transform;
            obj.transform.localScale = 2.5f*Vector3.one;
            var sprr = obj.AddComponent<SpriteRenderer>();
            sprr.sprite = sprite;
            obj.transform.position = startPos;

            obj.transform.DOMove(startPos + GetMovePos(i), 0.5f)
                .SetEase(Ease.OutCubic)
                .OnComplete(() =>
                {
                    sprr.DOFade(0, 0.3f).SetEase(Ease.Linear).OnComplete(()=>
                    {
                        Destroy(obj);
                    });
                });
        }

        private Vector3 GetMovePos(int i)
        {
            float angle = 0;
            switch (i)
            {
                case 0: angle = 170f; break;
                case 1: angle = 130f; break;
                case 2: angle = 90f; break;
                case 3: angle = 50f; break;
                case 4: angle = 10f; break;
                default: break;
            }

            return new Vector3(1f * Mathf.Cos(Mathf.Deg2Rad * angle), 1f * Mathf.Sin(Mathf.Deg2Rad * angle), 0);
        }

        private Sprite GetSprite(int i)
        {
            return i switch
            {
                0 => _image1,
                1 => _image2,
                2 => _image3,
                3 => _image4,
                4 => _image2,
                _ => null
            };
        }
    }
}