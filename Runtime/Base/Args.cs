using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public interface IArgs : IReference
    {
        IArgs SetObject(string paramName, object value);
        IArgs SetInt(string paramName, int value);
        IArgs SetFloat(string paramName, float value);
        IArgs SetBool(string paramName, bool value);
        IArgs SetString(string paramName, string value);

        T GetObject<T>(string paramName);
        int GetInt(string paramName);
        float GetFloat(string paramName);
        bool GetBool(string paramName);
        string GetString(string paramName);

        bool IsEmpty();
        bool IsDestroy { get; }
    }

    class ArgsPool : ObjectPool<Args>
    {
        static ArgsPool instance;

        protected override Args New()
        {
            return new Args();
        }

        public static ArgsPool Instance
        {
            get { return instance ?? (instance = new ArgsPool()); }
        }
    }
    public class Args : IArgs
    {
        public static Args Ctor()
        {
            return ArgsPool.Instance.Pop();
        }
        internal Args() { }

        Dictionary<string, object> objectArgs = new Dictionary<string, object>();
        Dictionary<string, int> intArgs = new Dictionary<string, int>();
        Dictionary<string, float> floatArgs = new Dictionary<string, float>();
        Dictionary<string, bool> boolArgs = new Dictionary<string, bool>();
        Dictionary<string, string> stringArgs = new Dictionary<string, string>();

        List<Type> cantSetToObjectTypes = new List<Type>
        {
            typeof(int),
            typeof(float),
            typeof(bool),
            typeof(string),
        };

        public bool IsDestroy { get; private set; }

        public bool IsUnused { get { return RefCount <= 0; } }

        public int RefCount { get; private set; }

        IArgs Set<T>(Dictionary<string, T> dic, string paramName, T value)
        {
            if (dic.ContainsKey(paramName))
            {
                dic[paramName] = value;
            }
            else
            {
                dic.Add(paramName, value);
            }

            IsDestroy = false;

            return this;
        }

        public IArgs SetObject(string paramName, object value)
        {
            var valueType = value.GetType();
            if (cantSetToObjectTypes.Contains(valueType))
            {
                throw new Exception($"value type is {valueType}, cant set to object list");
            }

            return Set(objectArgs, paramName, value);
        }

        public IArgs SetInt(string paramName, int value)
        {
            return Set(intArgs, paramName, value);
        }

        public IArgs SetFloat(string paramName, float value)
        {
            return Set(floatArgs, paramName, value);
        }

        public IArgs SetBool(string paramName, bool value)
        {
            return Set(boolArgs, paramName, value);
        }

        public IArgs SetString(string paramName, string value)
        {
            return Set(stringArgs, paramName, value);
        }


        T Get<T>(Dictionary<string, T> dic, string paramName)
        {
            if (IsDestroy == true)
            {
                Debug.LogErrorFormat("尝试获取一个已经被放回对象池的Args的参数");
                return default;
            }

            if (!dic.ContainsKey(paramName))
            {
                return default;
            }
            return dic[paramName];
        }


        /// <summary>
        /// 获取不是 int float bool string 之外的值
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="paramName">Parameter name.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public T GetObject<T>(string paramName)
        {
            return (T)Get(objectArgs, paramName);
        }

        public int GetInt(string paramName)
        {
            return Get(intArgs, paramName);
        }

        public float GetFloat(string paramName)
        {
            return Get(floatArgs, paramName);
        }

        public bool GetBool(string paramName)
        {
            return Get(boolArgs, paramName);
        }

        public string GetString(string paramName)
        {
            return Get(stringArgs, paramName);
        }

        public bool IsEmpty()
        {
            return objectArgs.Keys.Count == 0 && intArgs.Count == 0 && floatArgs.Count == 0 && boolArgs.Count == 0 && stringArgs.Count == 0;
        }

        string ToString<T>(Dictionary<string, T> dic)
        {
            string result = string.Empty;
            foreach (var kv in dic)
            {
                result = string.Format("{0} \n key=>{1}   value=>{2}", result, kv.Key, kv.Value);
            }
            return result;
        }

        public override string ToString()
        {
            string result = string.Empty;

            string objStr = ToString(objectArgs);
            string intStr = ToString(intArgs);
            string floatStr = ToString(floatArgs);
            string boolStr = ToString(boolArgs);
            string stringStr = ToString(stringArgs);

            result = string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n", objStr, intStr, floatStr, boolStr, stringStr);

            return result;
        }

        public void Retain()
        {
            RefCount++;
        }

        public void Release()
        {
            RefCount--;

            if (RefCount > 0)
            {
                return;
            }

            objectArgs.Clear();
            intArgs.Clear();
            floatArgs.Clear();
            boolArgs.Clear();
            stringArgs.Clear();

            IsDestroy = true;
            ArgsPool.Instance.Push(this);
        }
    }
}
