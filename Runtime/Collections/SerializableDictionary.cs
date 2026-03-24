using System;
using System.Collections.Generic;

using UnityEngine;

namespace Framework.Collections
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        public List<TKey> keys;
        [SerializeField]
        public List<TValue> values;

        public SerializableDictionary(Dictionary<TKey, TValue> dic)
        {
            keys = new List<TKey>(dic.Count);
            values = new List<TValue>(dic.Count);

            keys.AddRange(dic.Keys);
            values.AddRange(dic.Values);

            for (int i = 0; i < keys.Count; ++i)
            {
                this.Add(keys[i], values[i]);
            }
        }

        public void OnBeforeSerialize()
        {
            keys = new List<TKey>(this.Count);
            values = new List<TValue>(this.Count);
            foreach (var kvp in this)
            {
                keys.Add(kvp.Key);
                values.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            this.Clear();
            int count = Mathf.Min(keys.Count, values.Count);
            for (int i = 0; i < count; ++i)
            {
                this.Add(keys[i], values[i]);
            }
        }
    }
}