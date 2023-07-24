using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Mono.Cecil;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sabanihsi.ScreenSystem
{
    public class ScreenAnimation:MonoBehaviour
    {
        private static ScreenAnimation _instance;
        public static ScreenAnimation Instance => _instance;

        [SerializeField] private RectTransform _root;
        [SerializeField] private TMP_Text _stageTitle;
        [SerializeField] private PlayerImage _playerImage;

        private const float InPos = -2f;
        private const float CenterPos = 0;
        private const float OutPos = 2f;
        


        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// フェードアウト
        /// </summary>
        public async UniTask FadeOut(float time,CancellationToken token)
        {
            _root.position = new Vector3(CenterPos, 0, 0);
            await _root.DOMoveX(OutPos, time).SetEase(Ease.OutCubic).WithCancellation(token);
        }
        
        /// <summary>
        /// フェードイン
        /// </summary>
        public async UniTask FadeIn(string title,float time,CancellationToken token)
        {
            SetPanel(title);
            _root.position = new Vector3(InPos, 0, 0);
            await _root.DOMoveX(CenterPos, time).SetEase(Ease.OutCubic).WithCancellation(token);
        }

        private void SetPanel(string title)
        {
            if (Random.value > 0.5f)
            {
                _playerImage.StartBoxWalk();
            }
            else
            {
                _playerImage.StartPaintWalk();
            }
            
            _stageTitle.text = title;
            
        }
    }
}