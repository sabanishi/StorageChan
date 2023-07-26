using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Sabanihsi.ScreenSystem
{
    public class TitleScreen:AbstractScreen
    {
        [SerializeField]private RectTransform _textTransform;
        
        protected override void InitializeInternal(StageData data,CancellationToken token)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(_textTransform.DOLocalMoveY(-340f, 0.1f)
                .SetDelay(0.35f))
                .SetLoops(-1,LoopType.Yoyo)
                .ToUniTask(cancellationToken:this.GetCancellationTokenOnDestroy());
            sequence.Play();
        }

        private void Update()
        {
            if (Input.GetButtonDown("Decide"))
            {
                ScreenTransition.Instance.Move(ScreenEnum.MainGame).Forget();
            }
        }
        
        protected override  async UniTask<StageData> CloseInternal(CancellationToken token)
        {
            return new StageData("StageSelect");
        }
    }
}