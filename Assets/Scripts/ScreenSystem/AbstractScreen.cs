using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Sabanihsi.ScreenSystem
{
    /// <summary>
    /// Screenの基底クラス
    /// </summary>
    public class AbstractScreen:MonoBehaviour
    {
        /// <summary>
        /// Screen生成時の処理
        /// </summary>
        public void Initialize(Object data,CancellationToken token)
        {
            InitializeInternal(data, token);
        }

        /// <summary>
        /// Screen破棄時の処理
        /// </summary>
        /// <returns></returns>
        public Object Dispose()
        {
            return DisposeInternal();
        }

        /// <summary>
        /// Screenを開く際のアニメーション
        /// </summary>
        /// <param name="token"></param>
        public virtual async UniTask OpenAnimation(CancellationToken token)
        {
            
        }
        
        /// <summary>
        /// Screenを閉じる際のアニメーション
        /// </summary>
        /// <param name="token"></param>
        public virtual async UniTask CloseAnimation(CancellationToken token)
        {
            
        }

        public async UniTask Open(CancellationToken token)
        {
            await OpenInternal(token);
        }

        public async UniTask Close(CancellationToken token)
        {
            await CloseInternal(token);
        }

        /// <summary>
        /// Screen生成時の処理
        /// Override用
        /// </summary>
        /// <param name="data">初期化に必要な前Screenから渡されるデータ</param>
        /// <param name="token">cancellation token</param>
        /// <returns></returns>
        protected virtual void InitializeInternal(Object data, CancellationToken token)
        {
        }

        /// <summary>
        /// Screen破棄時の処理
        /// Override用
        /// </summary>
        protected virtual Object DisposeInternal()
        {
            return null;
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
        protected virtual UniTask CloseInternal(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }
    }
}