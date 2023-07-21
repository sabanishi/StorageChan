using Sabanishi.Common;
using Sabanishi.MainGame;
using UnityEngine;

namespace MainGame.Stage
{
    public class BlockChip:MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _sprr;
        [SerializeField] private GameObject _paintSignObj;
        [SerializeField] private bool _canPaint;
        [SerializeField]private PaintObjDict _paintObjDict;
        [SerializeField]private IsPaintDict _isPaintDict;

        public SpriteRenderer Sprr => _sprr;
        public bool CanPaint(Direction direction)
        {
            return _canPaint && !_isPaintDict.GetDict()[direction];
        }

        public void SetPaintSignActive(bool isActive)
        {
            _paintSignObj.SetActive(isActive);
        }

        /// <summary>
        /// 接地できるかを返す
        /// </summary>
        /// <returns></returns>
        public bool CanStick(Direction direction)
        {
            return _isPaintDict.GetDict()[direction];
        }

        /// <summary>
        /// 壁にペンキを塗る
        /// </summary>
        /// <param name="direction">塗られる方向</param>
        public void Paint(Direction direction)
        {
            _isPaintDict.GetDict()[direction] = true;
            _paintObjDict.GetDict()[direction].SetActive(true);
        }
    }
    
    [System.Serializable]
    public class PaintObjDict : BaseDictionary<Direction,GameObject, PaintObjPair> { }

    [System.Serializable]
    public class PaintObjPair : BaseKeyValuePair<Direction,GameObject>
    {
        public PaintObjPair(Direction dir,GameObject obj) : base(dir,obj) { }
    }
    
    [System.Serializable]
    public class IsPaintDict : BaseDictionary<Direction,bool, IsPaintPair> { }

    [System.Serializable]
    public class IsPaintPair : BaseKeyValuePair<Direction,bool>
    {
        public IsPaintPair(Direction dir,bool isPaint) : base(dir,isPaint) { }
    }
}