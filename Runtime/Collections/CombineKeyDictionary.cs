using System;
using System.Collections.Concurrent;

namespace Framework.Collections
{
    public class CombineKeyDictionary<TKey1, TKey2, TValue>
    {
        struct Key : IEquatable<Key>
        {
            public readonly TKey1 key1;
            public readonly TKey2 key2;
            public Key(TKey1 key1, TKey2 key2)
            {
                this.key1 = key1;
                this.key2 = key2;
            }

            public bool Equals(Key other)
            {
                return key1.Equals(other.key1) && key2.Equals(other.key2);
            }

            public override int GetHashCode()
            {
                return Utility.Hash.CombineHash(key1.GetHashCode(), key2.GetHashCode());
            }

        }

        ConcurrentDictionary<Key, TValue> dictionary = new ConcurrentDictionary<Key, TValue>();

        /// <summary>
        /// 判断是否包含某个key
        /// </summary>
        public bool ContainsKey(TKey1 key1, TKey2 key2)
        {
            return dictionary.ContainsKey(new Key(key1, key2));
        }

        /// <summary>
        /// 安全的添加一个值
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <param name="value"></param>
        public void TryAdd(TKey1 key1, TKey2 key2, TValue value)
        {
            dictionary.TryAdd(new Key(key1, key2), value);
        }

        /// <summary>
        /// 安全的返回一个值
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <returns></returns>
        public bool TryGet(TKey1 key1, TKey2 key2, out TValue value)
        {
            var key = new Key(key1, key2);
            if (dictionary.TryGetValue(key, out  value))
            {
                return true;
            }

            return false;
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

            dictionary.TryRemove(new Key(key1, key2), out _);
        }
    }
}
