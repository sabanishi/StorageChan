using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = System.Object;

namespace Sabanihsi.ScreenSystem
{
    /// <summary>
    /// Screenの基底クラス
    /// </summary>
    public class AbstractScreen:MonoBehaviour
    {
        protected StageData _stageData;
        /// <summary>
        /// Screen生成時の処理
        /// </summary>
        public void Initialize(StageData data,CancellationToken token)
        {
            InitializeInternal(data, token);
        }

        /// <summary>
        /// Screen破棄時の処理
        /// </summary>
        public void Dispose()
        {
            DisposeInternal();
        }

        /// <summary>
        /// Screenを開く際のアニメーション
        /// </summary>
        /// <param name="token"></param>
        public virtual async UniTask OpenAnimation(CancellationToken token)
        {
            await UniTask.Delay(1000, cancellationToken: token);
            await ScreenAnimation.Instance.FadeOut(0.4f,token);
        }
        
        /// <summary>
        /// Screenを閉じる際のアニメーション
        /// </summary>
        public virtual async UniTask CloseAnimation(StageData data,CancellationToken token)
        {
            SoundManager.PlaySE(SE_Enum.SCENECHANGE);
            await ScreenAnimation.Instance.FadeIn(data.StageName,0.4f,token);
        }

        public async UniTask Open(CancellationToken token)
        {
            await OpenInternal(token);
        }

        public async UniTask<StageData> Close(CancellationToken token)
        {
            return await CloseInternal(token);
        }

        /// <summary>
        /// Screen生成時の処理
        /// Override用
        /// </summary>
        /// <param name="data">初期化に必要な前Screenから渡されるデータ</param>
        /// <param name="token">cancellation token</param>
        /// <returns></returns>
        protected virtual void InitializeInternal(StageData data, CancellationToken token)
        {
        }

        /// <summary>
        /// Screen破棄時の処理
        /// Override用
        /// </summary>
        protected virtual void DisposeInternal()
        {
        }
        
        /// <summary>
        /// オープン処理
        /// Override用
        /// </summary>
        protected virtual UniTask OpenInternal(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }
        
        /// <summary>
        /// クローズ処理
        /// Override用
        /// </summary>
        protected virtual UniTask<StageData> CloseInternal(CancellationToken token)
        {
            return new UniTask<StageData>(null);
        }
    }
}