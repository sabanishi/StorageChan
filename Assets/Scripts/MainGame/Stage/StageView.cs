using DG.Tweening;
using MainGame.Stage;
using Sabanishi.Common;
using UniRx;
using UnityEngine;

namespace Sabanishi.MainGame
{
    public class StageView : MonoBehaviour
    {
        [SerializeField] private MapChipDict _mapChipDict;

        private Transform _root;

        private GameObject[,] _mapChipArray;

        public void Initialize(int width, int height)
        {
            _root = transform;
            _mapChipArray = new GameObject[width, height];
        }

        /// <summary>
        /// ModelのStageDataが変更されたときに呼ばれる
        /// </summary>
        /// <param name="replaceEvent"></param>
        public void OnStageChipReplaced(CollectionReplaceEvent<ChipData> replaceEvent)
        {
            int x = replaceEvent.NewValue.X;
            int y = replaceEvent.NewValue.Y;

            Debug.Log($"Replace:{x},{y},{replaceEvent.NewValue}");

            CreateMapChip(x, y, replaceEvent.NewValue);
        }

        /// <summary>
        /// ModelのStageDataに要素が追加されたときに呼ばれる
        /// </summary>
        public void OnStageChipAdded(CollectionAddEvent<ChipData> addEvent)
        {
            if (addEvent.Value == null)
            {
                return;
            }
            int x = addEvent.Value.X;
            int y = addEvent.Value.Y;

            CreateMapChip(x, y, addEvent.Value);
        }

        private void CreateMapChip(int x, int y, ChipData chipData)
        {
            if (_mapChipDict.GetDict().TryGetValue(chipData.ChipEnum, out var prefab))
            {
                var obj = Instantiate(prefab, _root);
                obj.transform.localPosition = new Vector3(x, y, 0);
                if (!chipData.ChipEnum.Equals(ChipEnum.IndoorBack))
                {
                    _mapChipArray[x, y] = obj;
                }
                if (chipData.ChipEnum is ChipEnum.CannotPaintBlock or ChipEnum.CanPaintBlock)
                {
                    obj.GetComponent<BlockChip>().Sprr.sprite = chipData.Image;
                }
                else if (chipData.ChipEnum is ChipEnum.Box)
                {
                    obj.GetComponent<Box>().Initialize(chipData);
                }
            }
        }

        /// <summary>
        /// ModelのStageDataから要素が削除された時に呼ばれる
        /// </summary>
        public void OnStageChipRemoved(CollectionRemoveEvent<ChipData> removeEvent)
        {
            int x = removeEvent.Value.X;
            int y = removeEvent.Value.Y;
            Destroy(_mapChipArray[x, y]);
            _mapChipArray[x, y] = null;
        }

        public void DropBox(ChipData chipData)
        {
            var obj = _mapChipArray[chipData.X, chipData.Y];
            if (obj == null) return;
            _mapChipArray[chipData.X, chipData.Y] = null;
            _mapChipArray[chipData.X, chipData.Y - 1] = obj;

            obj.transform.DOMoveY(obj.transform.position.y - 1, 0.3f).SetEase(Ease.InSine);
        }

        public GameObject GetBlock(int x, int y)
        {
            if (x < 0 || x >= _mapChipArray.GetLength(0) || y < 0 || y >= _mapChipArray.GetLength(1)) return null;
            return _mapChipArray[x, y];
        }
    }

    [System.Serializable]
    public class MapChipDict : BaseDictionary<ChipEnum, GameObject, MapChipPair>
    {
    }

    [System.Serializable]
    public class MapChipPair : BaseKeyValuePair<ChipEnum, GameObject>
    {
        public MapChipPair(ChipEnum chip, GameObject obj) : base(chip, obj)
        {
        }
    }
}