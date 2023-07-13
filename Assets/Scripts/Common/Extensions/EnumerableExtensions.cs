using System.Collections.Generic;
using System.Linq;

namespace Sabanishi.Common
{
    /// <summary>
    /// Enumerable型に関する拡張
    /// </summary>
    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source switch
            {
                null => true,
                ICollection<T> collection => collection.Count == 0,
                IReadOnlyCollection<T> readOnlyCollection => readOnlyCollection.Count == 0,
                _ => !source.Any()
            };
        }
        
        public static bool IsOutOfRange<T>(this IList<T> source, int index)
        {
            return source.IsNullOrEmpty() || index < 0 || index >= source.Count;
        }

        public static bool TryGet<T>(this IList<T> source, int index, out T element)
        {
            element = default;
            if (source.IsNullOrEmpty()) return false;
            if (source.IsOutOfRange(index)) return false;
            
            element=source[index];
            return true;
        }
        
        
    }
}