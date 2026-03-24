using System;
using System.Collections.Generic;

namespace FInject
{
    /// <summary>
    /// 依赖注入器
    /// </summary>
    public static class Bootstrapper
    {
        static List<(Type, object)> injectedCache = new List<(Type, object)>();
        static Container container;

        /// <summary>
        /// 上下文 会按照新的context的注入信息重新注入 新context中没有的不会改变 通过构造方法注入的不会更改
        /// </summary>
        public static Container Container 
        {
            get { return container; }
            set 
            {
                if (value == null)
                {
                    throw new ArgumentNullException("container");
                }
                
                if(value == container)
                {
                    return;
                }
                container = value;

                for (int i = 0; i < injectedCache.Count; i++)
                {
                    var injected = injectedCache[i];
                    Inject(injected.Item1, injected.Item2);
                }
            } 
        }

        /// <summary>
        /// 注入的入口
        /// </summary>
        /// <param name="type">为哪个类型注入</param>
        /// <param name="instance">要注入的实例</param>
        static void Inject(Type type, object instance)
        {
            type = instance == null ? type : instance.GetType();
            InjectWithFields(type, instance);
            InjectWithMethod(type, instance);
            InjectWithPropertys(type, instance);
        }

        /// <summary>
        /// 根据实例注入
        /// </summary>
        /// <typeparam name="T1">实例类型</typeparam>
        /// <param name="instance">实例</param>
        public static void Inject<T1>(T1 instance)
        {
            Inject(typeof(T1), instance);
        }

        /// <summary>
        /// 根据实例注入
        /// </summary>
        /// <param name="instance">实例</param>
        public static void Inject(object instance)
        {
            Inject(instance.GetType(), instance);
        }

        /// <summary>
        /// 为静态类注入
        /// </summary>
        /// <param name="type">静态类型</param>
        public static void Inject(Type type)
        {
            if (!type.IsStatic())
            {
                throw new Exception($"type {type.FullName} is not static");
            }
            Inject(type, null);
        }

        /// <summary>
        /// 根据类型创建实例同时进行注入
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>实例</returns>
        public static object CreateInstance(Type type)
        {
            var instance = InjectWithConstructor(type);
            if (instance == null)
            {
                instance = Activator.CreateInstance(type);
            }

            Inject(instance);
            return instance;
        }

        /// <summary>
        /// 根据类型创建实例同时进行注入
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="args">参数</param>
        /// <returns>实例</returns>
        public static object CreateInstance(Type type, params object[] args)
        {
            var instance = InjectWithConstructor(type);
            if(instance == null)
            {
                instance = Activator.CreateInstance(type, args);
            }

            Inject(instance);
            return instance;
        }

        /// <summary>
        /// 根据类型创建实例同时进行注入
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <returns>实例</returns>
        public static T CreateInstance<T>()
        {
            var type = typeof(T);
            return (T)CreateInstance(type);
        }

        /// <summary>
        /// 根据类型创建实例同时进行注入
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="args">参数</param>>
        /// <returns>实例</returns>
        public static T CreateInstance<T>(params object [] args)
        {
            var type = typeof(T);
            return (T)CreateInstance(type, args);
        }

        /// <summary>
        /// 根据类型创建实例同时进行注入
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <returns>实例</returns>
        public static T CreateInstanceWithNew<T>() where T : new()
        {
            var instance = new T();
            Inject(instance);
            return instance;
        }

        /// <summary>
        /// 根据绑定信息创建实例
        /// </summary>
        /// <param name="bindInfo"></param>
        /// <returns></returns>
        static object Create(BindInfo bindInfo)
        {
            var instance = bindInfo.instance ?? CreateInstance(bindInfo.bindType);
            Inject(instance);
            return instance;
        }

        /// <summary>
        /// 替换
        /// </summary>
        /// <param name="type"></param>
        /// <param name="instance"></param>
        static void CacheInjected(Type type, object instance = null)
        {
            bool replace = false;
            for (int i = 0; i < injectedCache.Count; i++)
            {
                var injected = injectedCache[i];
                if (type == injected.Item1 && instance == injected.Item2)
                {
                    injectedCache[i] = (type, instance);
                    replace = true;
                    break;
                }
            }

            if (!replace)
            {
                injectedCache.Add((type, instance));
            }
        }

        /// <summary>
        /// 通过属性注入
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="instance">实例</param>
        static void InjectWithPropertys(Type type, object instance = null)
        {
            foreach(var propertyInfo in Cache.GetPropertyInfos(type))
            {
                var bindInfo = container.GetBindInfo(propertyInfo.PropertyType, type);
                if (bindInfo == null || bindInfo.IsEmpty())
                {
                    continue;
                }

                var owner = propertyInfo.IsStatic() ? type : instance;
                propertyInfo.SetValue(owner, Create(bindInfo));
            }

            CacheInjected(type, instance);
        }

        /// <summary>
        /// 通过字段注入
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="instance">实例</param>
        static void InjectWithFields(Type type, object instance = null)
        {
            foreach(var fieldInfo in type.GetFieldInfos())
            {
                var bindInfo = container.GetBindInfo(fieldInfo.FieldType, type);
                if (bindInfo == null || bindInfo.IsEmpty())
                {
                    continue;
                }

                var owner = fieldInfo.IsStatic ? type : instance;
                fieldInfo.SetValue(owner, Create(bindInfo));
            }

            CacheInjected(type, instance);
        }

        /// <summary>
        /// 通过方法注入
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="instance">实例</param>
        static void InjectWithMethod(Type type, object instance = null)
        {
            foreach(var methodInfo in type.GetMethodInfos())
            {
                var parameterInfos = methodInfo.GetParameters();
                var bindInfo = container.GetBindInfo(parameterInfos[0].ParameterType, type);
                if (bindInfo == null || bindInfo.IsEmpty())
                {
                    continue;
                }

                var owner = methodInfo.IsStatic ? type : instance;
                methodInfo.Invoke(owner, new object[] { Create(bindInfo) });
            }

            CacheInjected(type, instance);
        }

        /// <summary>
        /// 通过构造方法注入
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>注入后的实例</returns>
        static object InjectWithConstructor(Type type)
        {
            var ctorInfo = type.GetConstructorInfo();
            if(ctorInfo == null)
            {
                return null;
            }
            var parameterInfos = ctorInfo.GetParameters();
            var bindInfo = container.GetBindInfo(parameterInfos[0].ParameterType, type);
            if (bindInfo == null || bindInfo.IsEmpty())
            {
                return null;
            }

            return ctorInfo.Invoke(new object[] { Create(bindInfo) });
        }

        /// <summary>
        /// 通过这个创建的实例在不使用的时候需要通过这个来进行释放 否则引用会一直存在
        /// </summary>
        /// <param name="instance">实例</param>
        public static void Release(object instance)
        {
            for(int i = 0; i < injectedCache.Count; i++)
            {
                var data = injectedCache[i];
                if(data.Item2 == instance)
                {
                    injectedCache.Remove(data);
                }
            }
        }

        /// <summary>
        /// 释放所有的 已经注入的不会更改
        /// </summary>
        public static void Release()
        {
            injectedCache.Clear();
        }
    }
}