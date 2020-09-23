using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Framework.Utility
{
    public static class InstanceFactory
    {
        static Assembly _assembly;

        static Assembly assembly
        {
            get
            {
                return _assembly ?? (_assembly = Assembly.GetExecutingAssembly());
            }
        }

        static Type[] types;

        static Type[] Types
        {
            get
            {
                return types ?? (types = assembly.GetTypes());
            }
        }

        static Dictionary<string, Type> TypeNames = new Dictionary<string, Type>();
        

        static bool Conditional(Type type, Type baseType, Type interfaceType)
        {
            if (type.IsAbstract || type.IsNotPublic)
            {
                return false;
            }

            if (baseType != null && (type.BaseType == null || type.BaseType != baseType))
            {
                return false;
            }

            if (interfaceType != null && interfaceType.IsInterface && type.GetInterface(interfaceType.Name) == null)
            {
                return false;
            }

            return true;
        }

        public static List<T> CreateInstances<T>(Type baseType, Type interfaceType = null, bool nonPublic = false)
        {
            List<T> ret = new List<T>();
            foreach (var type in Types)
            {
                if (!Conditional(type, baseType, interfaceType))
                {
                    continue;
                }

                object instance = Activator.CreateInstance(type, nonPublic);
                try
                {
                    ret.Add((T)instance);
                }
                catch
                {

                }
            }
            if (ret.Count == 0)
            {
                Debug.LogWarningFormat("没有满足这些条件的Instances  baseType = {0}  interfaceType = {1}", baseType, interfaceType);
                return null;
            }
            return ret;
        }

        public static T CreateInstance<T>(string typeName, bool nonPulic = false)
        {
            Type type = GetTypeByName(typeName);

            object instance = Activator.CreateInstance(type, nonPulic);

            if (instance is T)
            {
                return (T)instance;
            }

            Debug.LogWarningFormat("创建Instance失败   typeName=>{0}", typeName);
            return default;
        }

        public static Type GetTypeByName(string typeName)
        {
            if (TypeNames.ContainsKey(typeName))
            {
                return TypeNames[typeName];
            }

            foreach (var type in Types)
            {
                if (type.Name == typeName)
                {
                    TypeNames.Add(typeName, type);
                    return type;
                }
            }

            return null;
        }
    }
}
