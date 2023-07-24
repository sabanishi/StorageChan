using System;
using Cysharp.Threading.Tasks;
using Sabanihsi.ScreenSystem;
using TMPro;
using UnityEngine;

namespace Sabanishi.MainGame
{
    public class ResultPanel:MonoBehaviour
    {
        private const string TimeFormat = @"hh\:mm\:ss";
        [SerializeField] private GameObject _root;
        [SerializeField] private GameObject _timeRoot;
        [SerializeField] private GameObject _paintRoot;
        [SerializeField] private TMP_Text _timeText;
        [SerializeField] private TMP_Text _paintText;
        [SerializeField] private GameObject _explainRoot;
        
        public void Show(TimeSpan time,int paintCount)
        {
            UniTask.Void(async () =>
            {
                _root.SetActive(true);
                _timeRoot.SetActive(false);
                _paintRoot.SetActive(false);
                _explainRoot.SetActive(false);
                _timeText.text = time.ToString(TimeFormat);
                _paintText.text = paintCount.ToString();
                
                await UniTask.Delay(TimeSpan.FromSeconds(0.6f));
                
                _timeRoot.SetActive(true);
                
                await UniTask.Delay(TimeSpan.FromSeconds(0.6f));
                
                _paintRoot.SetActive(true);
                
                await UniTask.Delay(TimeSpan.FromSeconds(0.6f));
                
                _explainRoot.SetActive(true);
                
                //Enterキーが押されるまで待機
                await UniTask.WaitUntil(() => Input.GetButtonDown("Decide"));
                 
                ScreenTransition.Instance.Move(ScreenEnum.Home).Forget();
            });
        }
        
    }
}