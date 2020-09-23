using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Collections
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> _keys;
        [SerializeField]
        private List<TValue> _values;

        public void OnBeforeSerialize()
        {
            _keys = new List<TKey>(this.Count);
            _values = new List<TValue>(this.Count);
            foreach (var kvp in this)
            {
                _keys.Add(kvp.Key);
                _values.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            this.Clear();
            int count = Mathf.Min(_keys.Count, _values.Count);
            for (int i = 0; i < count; ++i)
            {
                this.Add(_keys[i], _values[i]);
            }
        }
    }


    [Serializable]
    public class KVData_string_string : SerializableDictionary<string, string> { }

    [Serializable]
    public class KVData_int_string : SerializableDictionary<int, string> { }

    [Serializable]
    public class KVData_string_int : SerializableDictionary<string, int> { }

    [Serializable]
    public class KVData_int_int : SerializableDictionary<int, int> { }

    [Serializable]
    public class KVData_int_float : SerializableDictionary<int, float> { }

    [Serializable]
    public class KVData_int_Vector3 : SerializableDictionary<int, Vector3> { }

    [Serializable]
    public class KVData_int_KVData_int_Vector3 : SerializableDictionary<int, KVData_int_Vector3> { }
}