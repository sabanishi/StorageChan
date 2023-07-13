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
        
        private int _width;
        private int _height;
        
        public void Initialize(int width,int height)
        {
            _width = width;
            _height= height;
            _root = transform;
            _mapChipArray= new GameObject[_width, _height];
        }

        /// <summary>
        /// ModelのStageDataが変更されたときに呼ばれる
        /// </summary>
        /// <param name="replaceEvent"></param>
        public void OnStageChipReplaced(CollectionReplaceEvent<ChipEnum> replaceEvent)
        {
            int x = replaceEvent.Index % _width;
            int y = replaceEvent.Index / _width;

            Debug.Log($"Replace:{x},{y},{replaceEvent.NewValue}");
            
            CreateMapChip(x,y,replaceEvent.NewValue);
        }
        
        /// <summary>
        /// ModelのStageDataに要素が追加されたときに呼ばれる
        /// </summary>
        /// <param name="addEvent"></param>
        public void OnStageChipAdded(CollectionAddEvent<ChipEnum> addEvent)
        {
            int x = addEvent.Index % _width;
            int y = addEvent.Index / _width;
            Debug.Log($"Add:{x},{y},{addEvent.Value}");
            
            CreateMapChip(x,y,addEvent.Value);
        }

        private void CreateMapChip(int x,int y,ChipEnum chipEnum)
        {
            if (_mapChipDict.GetDict().TryGetValue(chipEnum, out var prefab))
            {
                var obj = Instantiate(prefab, _root);
                obj.transform.localPosition = new Vector3(x, y, 0);
                _mapChipArray[x, y] = obj;
            }
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