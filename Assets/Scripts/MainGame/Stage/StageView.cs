using MainGame.Stage;
using Sabanishi.Common;
using UniRx;
using UnityEngine;

namespace Sabanishi.MainGame
{
    public class StageView:MonoBehaviour
    {
        [SerializeField]private MapChipDict _mapChipDict;
        
        private Transform _root;
        public Transform Root => _root;
        
        private GameObject[,] _mapChipArray;

        public void Initialize(int width,int height)
        {
            _root = transform;
            _mapChipArray= new GameObject[width, height];
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
            
            CreateMapChip(x,y,replaceEvent.NewValue);
        }
        
        /// <summary>
        /// ModelのStageDataに要素が追加されたときに呼ばれる
        /// </summary>
        public void OnStageChipAdded(CollectionAddEvent<ChipData> addEvent)
        {
            int x = addEvent.Value.X;
            int y = addEvent.Value.Y;

            CreateMapChip(x,y,addEvent.Value);
        }

        private void CreateMapChip(int x,int y,ChipData chipData)
        {
            if (_mapChipDict.GetDict().TryGetValue(chipData.ChipEnum, out var prefab))
            {
                var obj = Instantiate(prefab, _root);
                obj.transform.localPosition = new Vector3(x, y, 0);
                _mapChipArray[x, y] = obj;
                if (chipData.ChipEnum is ChipEnum.CannotPaintBlock or ChipEnum.CanPaintBlock)
                {
                    obj.GetComponent<BlockChip>().Sprr.sprite = chipData.Image;
                }else if (chipData.ChipEnum is ChipEnum.Box)
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
            int x= removeEvent.Value.X;
            int y = removeEvent.Value.Y;
            Destroy(_mapChipArray[x, y]);
            _mapChipArray[x,y] = null;
        }
    }
    
    [System.Serializable]
    public class MapChipDict : BaseDictionary<ChipEnum,GameObject, MapChipPair> { }

    [System.Serializable]
    public class MapChipPair : BaseKeyValuePair<ChipEnum,GameObject>
    {
        public MapChipPair(ChipEnum chip,GameObject obj) : base(chip,obj) { }
    }
}