using System;
using Cysharp.Threading.Tasks;
using Sabanihsi.ScreenSystem;
using Sabanishi.MainGame;
using UnityEngine;

namespace MainGame.Stage
{
    public class Flag:MonoBehaviour
    {
        [SerializeField] private GameObject _ballonObj;
        private string _stageName;
        private bool _isActive;
        private bool _canOperate;
        
        public void Initialize(int stageNum)
        {
            var stageName = $"Stage{stageNum}";
            _stageName = stageName;
            _canOperate = true;
            _isActive = false;
        }

        private void Update()
        {
            if (!_canOperate) return;
            if (_isActive && Input.GetButtonDown("Decide"))
            {
                var stageData = new StageData(_stageName);
                GameObject.FindWithTag("MainGameScreen").GetComponent<MainGameScreen>().SetStageData(stageData);
                ScreenTransition.Instance.Move(ScreenEnum.MainGame).Forget();
                _canOperate = false;
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _isActive = true;
                _ballonObj.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _isActive = false;
                _ballonObj.SetActive(false);
            }
        }
    }
}