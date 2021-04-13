using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Framework.Collections
{
    public class CombineKeyDictionary<TKey1, TKey2, TValue>
    {
        ConcurrentDictionary<TKey1, ConcurrentDictionary<TKey2, TValue>> Dic = new ConcurrentDictionary<TKey1, ConcurrentDictionary<TKey2, TValue>>();

        public bool ContainsKey(TKey1 key1)
        {
            return Dic.ContainsKey(key1);
        }

        public bool ContainsKey(TKey1 key1, TKey2 key2)
        {
            return ContainsKey(key1) && Dic[key1].ContainsKey(key2);
        }

        /// <summary>
        /// 安全的添加一个值
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <param name="value"></param>
        public void TryAdd(TKey1 key1, TKey2 key2, TValue value)
        {
            if (!ContainsKey(key1))
            {
                ConcurrentDictionary<TKey2, TValue> _dic = new ConcurrentDictionary<TKey2, TValue>();
                _dic.TryAdd(key2, value);
                Dic.TryAdd(key1, _dic);
            }
            else
            {
                Dic[key1].TryAdd(key2, value);
            }
        }

        /// <summary>
        /// 安全的返回一个字典
        /// </summary>
        /// <param name="key1"></param>
        /// <returns></returns>
        public ConcurrentDictionary<TKey2, TValue>Get(TKey1 key1)
        {
            Dic.TryGetValue(key1, out ConcurrentDictionary<TKey2, TValue> outer);
            return outer;
        }

        /// <summary>
        /// 安全的返回一个值
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <returns></returns>
        public TValue Get(TKey1 key1, TKey2 key2)
        {
            ConcurrentDictionary<TKey2, TValue> outers = Get(key1);
            if (outers == null)
            {
                return default;
            }
            outers.TryGetValue(key2, out TValue outer);
            return outer;
        }

        /// <summary>
        /// 安全的移除一组值
        /// </summary>
        /// <param name="key1"></param>
        public void Remove(TKey1 key1)
        {
            Dic.TryRemove(key1, out ConcurrentDictionary<TKey2, TValue> outer);
        }

        /// <summary>
        /// 安全的移除单个值
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        public void Remove(TKey1 key1, TKey2 key2)
        {
            if (!ContainsKey(key1, key2))
            {
                return;
            }
            Dic[key1].TryRemove(key2, out TValue outer);

            if(Dic[key1].Count == 0)
            {
                Dic.TryRemove(key1, out ConcurrentDictionary<TKey2, TValue> outer2);
            }
        }
    }
}
