using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Utility
{
    public static class StringUtility
    {
        static Dictionary<long, string> cache = new Dictionary<long, string>();

        public static string GetOrAttach(string left, string right)
        {
            long key = (left.GetHashCode() << 32) + right.GetHashCode();
            bool get = cache.TryGetValue(key, out string result);
            if (get)
            {
                return result;
            }

            result = left + right;
            cache.Add(key, result);
            return result;
        }
    }

}
