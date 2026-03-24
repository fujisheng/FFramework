using System;
using System.Collections.Generic;
using System.Reflection;

namespace Framework.IoC
{
    /// <summary>
    /// 反射信息缓存，提升性能
    /// </summary>
    internal static class ReflectionCache
    {
        static readonly BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.GetField | 
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic;
        
        static readonly Dictionary<Type, List<FieldInfo>> fieldCache = new Dictionary<Type, List<FieldInfo>>();
        static readonly Dictionary<Type, List<MethodInfo>> methodCache = new Dictionary<Type, List<MethodInfo>>();
        static readonly Dictionary<Type, List<ConstructorInfo>> ctorCache = new Dictionary<Type, List<ConstructorInfo>>();
        static readonly Dictionary<Type, List<PropertyInfo>> propertyCache = new Dictionary<Type, List<PropertyInfo>>();
        
        static readonly Type injectAttributeType = typeof(InjectAttribute);

        /// <summary>
        /// 获取添加了 [Inject] 特性的字段
        /// </summary>
        internal static List<FieldInfo> GetInjectFields(this Type type)
        {
            if (fieldCache.TryGetValue(type, out var fieldInfos))
            {
                return fieldInfos;
            }

            fieldInfos = new List<FieldInfo>();
            foreach (var fieldInfo in type.GetFields(bindingFlags))
            {
                if (!fieldInfo.IsDefined(injectAttributeType, true))
                {
                    continue;
                }

                fieldInfos.Add(fieldInfo);
            }
            
            fieldCache[type] = fieldInfos;
            return fieldInfos;
        }

        /// <summary>
        /// 获取添加了 [Inject] 特性的方法
        /// </summary>
        internal static List<MethodInfo> GetInjectMethods(this Type type)
        {
            if (methodCache.TryGetValue(type, out var methodInfos))
            {
                return methodInfos;
            }

            methodInfos = new List<MethodInfo>();
            foreach (var methodInfo in type.GetMethods(bindingFlags))
            {
                if (!methodInfo.IsDefined(injectAttributeType, true))
                {
                    continue;
                }

                // 支持多参数方法注入
                methodInfos.Add(methodInfo);
            }
            
            methodCache[type] = methodInfos;
            return methodInfos;
        }

        /// <summary>
        /// 获取添加了 [Inject] 特性的属性
        /// </summary>
        internal static List<PropertyInfo> GetInjectProperties(this Type type)
        {
            if (propertyCache.TryGetValue(type, out var propertyInfos))
            {
                return propertyInfos;
            }

            propertyInfos = new List<PropertyInfo>();
            foreach (var propertyInfo in type.GetProperties(bindingFlags))
            {
                if (!propertyInfo.IsDefined(injectAttributeType, true))
                {
                    continue;
                }
                propertyInfos.Add(propertyInfo);
            }
            
            propertyCache[type] = propertyInfos;
            return propertyInfos;
        }

        /// <summary>
        /// 获取添加了 [Inject] 特性的构造器
        /// </summary>
        internal static List<ConstructorInfo> GetInjectConstructors(this Type type)
        {
            if (ctorCache.TryGetValue(type, out var ctorInfos))
            {
                return ctorInfos;
            }

            ctorInfos = new List<ConstructorInfo>();
            foreach (var ctorInfo in type.GetConstructors(bindingFlags))
            {
                if (!ctorInfo.IsDefined(injectAttributeType, true))
                {
                    continue;
                }

                // 支持多参数构造器注入
                ctorInfos.Add(ctorInfo);
            }
            
            ctorCache[type] = ctorInfos;
            return ctorInfos;
        }

        /// <summary>
        /// 清空所有缓存
        /// </summary>
        internal static void ClearCache()
        {
            fieldCache.Clear();
            methodCache.Clear();
            ctorCache.Clear();
            propertyCache.Clear();
        }
    }
}
