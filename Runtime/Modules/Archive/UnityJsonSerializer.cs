using System;
using System.Text;

using UnityEngine;

namespace Framework.Module.Archive
{
    public class UnityJsonSerializer : ISerializer
    {
        public byte[] Serialize<T>(T obj)
        {
            return Encoding.UTF8.GetBytes(JsonUtility.ToJson(obj));
        }

        public byte[] Serialize(object obj)
        {
            return Encoding.UTF8.GetBytes(JsonUtility.ToJson(obj));
        }


        public T Deserialize<T>(byte[] data)
        {
            return JsonUtility.FromJson<T>(Encoding.UTF8.GetString(data));
        }

        public object Deserialize(byte[] data , Type type)
        {
            return JsonUtility.FromJson(Encoding.UTF8.GetString(data), type);
        }
    }
}