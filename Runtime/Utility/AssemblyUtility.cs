﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Framework
{
    public static partial class Utility
    {
        public static class Assembly
        {
            public const BindingFlags AllFlag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;
            private static readonly System.Reflection.Assembly[] Assemblies = null;
            private static readonly Dictionary<string, Type> CachedTypes = new Dictionary<string, Type>(StringComparer.Ordinal);

            static Assembly()
            {
                Assemblies = AppDomain.CurrentDomain.GetAssemblies();
            }

            /// <summary>
            /// 获取已加载的程序集。
            /// </summary>
            /// <returns>已加载的程序集。</returns>
            public static System.Reflection.Assembly[] GetAssemblies()
            {
                return Assemblies;
            }

            /// <summary>
            /// 获取已加载的程序集中的所有类型。
            /// </summary>
            /// <returns>已加载的程序集中的所有类型。</returns>
            public static Type[] GetTypes()
            {
                List<Type> results = new List<Type>();
                foreach (System.Reflection.Assembly assembly in Assemblies)
                {
                    results.AddRange(assembly.GetTypes());
                }

                return results.ToArray();
            }

            /// <summary>
            /// 获取已加载的程序集中的所有类型。
            /// </summary>
            /// <param name="results">已加载的程序集中的所有类型。</param>
            public static void GetTypes(List<Type> results)
            {
                Assert.IfNull(results, new Exception("Results is invalid."));

                results.Clear();
                foreach (System.Reflection.Assembly assembly in Assemblies)
                {
                    results.AddRange(assembly.GetTypes());
                }
            }

            /// <summary>
            /// 获取已加载的程序集中的指定类型。
            /// </summary>
            /// <param name="typeName">要获取的类型名。</param>
            /// <returns>已加载的程序集中的指定类型。</returns>
            public static Type GetType(string typeName)
            {
                Assert.IfIsNullOrEmpty(typeName, new Exception("Type name is invalid."));

                Type type = null;
                if (CachedTypes.TryGetValue(typeName, out type))
                {
                    return type;
                }

                type = Type.GetType(typeName);
                if (type != null)
                {
                    CachedTypes.Add(typeName, type);
                    return type;
                }

                foreach (System.Reflection.Assembly assembly in Assemblies)
                {
                    type = Type.GetType(string.Format("{0}, {1}", typeName, assembly.FullName));
                    if (type != null)
                    {
                        CachedTypes.Add(typeName, type);
                        return type;
                    }
                }

                return null;
            }

            /// <summary>
            /// 获取当前程序集内部继承自某个类型的类型
            /// </summary>
            /// <param name="baseType"></param>
            /// <returns></returns>
            public static Type GetAssignableType(Type baseType)
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                    .Where(item=>baseType.IsAssignableFrom(item))
                    .FirstOrDefault();
            }
        }
    }
}