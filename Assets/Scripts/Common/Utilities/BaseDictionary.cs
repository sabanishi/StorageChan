using System.Collections.Generic;
using UnityEngine;

namespace Sabanishi.Common
{
    /// <summary>
    /// コンソール上でDictionaryらしきものを使うために必要なクラス
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TType">Key,Valueの組み</typeparam>
    [System.Serializable]
    public class BaseDictionary<TKey, TValue, TType> where TType : BaseKeyValuePair<TKey, TValue>
    {
        [SerializeField]
        private List<TType> list;
        private Dictionary<TKey, TValue> _dict;
        
        public Dictionary<TKey, TValue> GetDict()
        {
            if (_dict == null)
            {
                _dict = ConvertListToDictionary(list);
            }
            return _dict;
        }

        public List<TType> GetList()
        {
            return list;
        }

        private static Dictionary<TKey, TValue> ConvertListToDictionary(List<TType> list)
        {
            Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();
            foreach (TType pair in list)
            {
                dict.Add(pair.Key, pair.Value);
            }
            return dict;
        }
    }
}