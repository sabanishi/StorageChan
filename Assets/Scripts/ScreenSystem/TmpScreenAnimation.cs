using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Mono.Cecil;
using UnityEngine;
using UnityEngine.UI;

namespace Sabanihsi.ScreenSystem
{
    public class TmpScreenAnimation:MonoBehaviour
    {
        [SerializeField] private Image _image;
        private static TmpScreenAnimation _instance;
        public static TmpScreenAnimation Instance => _instance;

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
            await _image.DOFade(0,time).ToUniTask(TweenCancelBehaviour.KillAndCancelAwait,token);
        }
        
        /// <summary>
        /// フェードイン
        /// </summary>
        public async UniTask FadeIn(float time,CancellationToken token)
        {
            await _image.DOFade(1,time).ToUniTask(TweenCancelBehaviour.KillAndCancelAwait,token);
        }
    }
}