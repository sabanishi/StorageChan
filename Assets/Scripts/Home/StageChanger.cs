using Cysharp.Threading.Tasks;
using Sabanihsi.ScreenSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Sabanishi.Home
{
    public class StageChanger:MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private string _stageName;
        
        private void Start()
        {
            _button.onClick.AddListener(() =>
            {
                var stageData = new StageData(_stageName);
                GameObject.FindWithTag("HomeScreen").GetComponent<HomeScreen>().SetStageData(stageData);
                ScreenTransition.Instance.Move(ScreenEnum.MainGame).Forget();
            });
        }
    }
}