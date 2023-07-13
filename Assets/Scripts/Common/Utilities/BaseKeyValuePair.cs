using System.Collections.Generic;

namespace Sabanishi.Common
{
    /// <summary>
    /// BaseDictionaryのキー、バリューの組み合わせ
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public abstract class BaseKeyValuePair<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;

        protected BaseKeyValuePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }
}