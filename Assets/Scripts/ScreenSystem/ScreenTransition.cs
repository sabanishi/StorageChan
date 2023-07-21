using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Sabanihsi.ScreenSystem
{
    public class ScreenTransition:MonoBehaviour
    {
        [SerializeField] private ScreenEnum _loadScreenEnum;

        private static ScreenTransition _instance;
        public static ScreenTransition Instance => _instance;
        
        /// <summary>
        /// 画面遷移中ならばtrue
        /// </summary>
        private bool _isTransitioning;

        private Dictionary<ScreenEnum, string> _screenPathDict = new()
        {
            { ScreenEnum.Home, "Screen/HomeScreen" },
            { ScreenEnum.MainGame, "Screen/MainGameScreen" }
        };

        private AbstractScreen _nowScreen;

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

        private void Start()
        {
            Move(_loadScreenEnum).Forget();
        }

        /// <summary>
        /// 次の画面に遷移する
        /// </summary>
        public async UniTask Move(ScreenEnum nextScreenEnum)
        {
            if (_isTransitioning) return;

            _isTransitioning = true;
            var token = this.GetCancellationTokenOnDestroy();
            Object data = null;

            if (_nowScreen != null)
            {
                await _nowScreen.Close(token);
                await _nowScreen.CloseAnimation(token);
                data = _nowScreen.Dispose();
                Destroy(_nowScreen.gameObject);
            }
            
            AbstractScreen nextScreen = null;
            bool isGetPrefab = TryGetScreenPrefab(nextScreenEnum, out var screenPrefab);
            if (isGetPrefab)
            {
                var nextScreenObj = Instantiate(screenPrefab);
                if (!nextScreenObj.TryGetComponent(out nextScreen))
                {
                    Debug.LogError("[ScreenTransition] AbstractScreenを継承したコンポーネントがアタッチされていません");
                }
            }

            if (nextScreen != null)
            {
                _nowScreen = nextScreen;
                //自身またはnowScreenが破壊されたい際に停止するトークンを生成
                var nextScreenCt = nextScreen.gameObject.GetCancellationTokenOnDestroy();
                var nextScreenCts = CancellationTokenSource.CreateLinkedTokenSource(token, nextScreenCt);
                nextScreen.Initialize(data, nextScreenCts.Token);
                await nextScreen.OpenAnimation(nextScreenCts.Token);
                nextScreen.Open(nextScreenCts.Token).Forget();
            }

            _isTransitioning = false;
        }

        /// <summary>
        /// スクリーンプレハブの取得を試みる
        /// </summary>
        private bool TryGetScreenPrefab(ScreenEnum screenEnum, out GameObject screenPrefab)
        {
            screenPrefab = null;
            if (!_screenPathDict.TryGetValue(screenEnum, out var path))
            {
                Debug.LogError("[ScreenTransition] ScreenEnumに対応するパスが見つかりませんでした");
                return false;
            }

            screenPrefab = Resources.Load<GameObject>(path);
            if (screenPrefab == null)
            {
                Debug.LogError("[ScreenTransition] ScreenPrefabの取得に失敗しました");
                return false;
            }

            return true;
        }
    }
}