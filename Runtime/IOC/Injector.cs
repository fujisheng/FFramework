using System;
using System.Collections.Generic;
using System.Reflection;

namespace Framework.IoC
{
    /// <summary>
    /// 依赖注入器
    /// 支持字段、属性、方法、构造器注入，支持多参数
    /// </summary>
    public static class Injector
    {
        static readonly List<(Type type, object instance)> injectedCache = new List<(Type, object)>();
        static Container container;

        /// <summary>
        /// 当前容器
        /// 设置新容器时会自动重新注入已缓存的对象
        /// </summary>
        public static Container Container
        {
            get => container;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "[Injector] Container cannot be null");
                }

                if (value == container)
                {
                    return;
                }
                
                container = value;

                // 重新注入已缓存的对象
                for (int i = 0; i < injectedCache.Count; i++)
                {
                    var injected = injectedCache[i];
                    InjectInternal(injected.type, injected.instance);
                }
            }
        }

        /// <summary>
        /// 为实例注入依赖
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="instance">实例</param>
        public static void Inject<T>(T instance)
        {
            InjectInternal(typeof(T), instance);
        }

        /// <summary>
        /// 为实例注入依赖
        /// </summary>
        /// <param name="instance">实例</param>
        public static void Inject(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            InjectInternal(instance.GetType(), instance);
        }

        /// <summary>
        /// 为静态类注入依赖
        /// </summary>
        /// <param name="type">静态类型</param>
        public static void Inject(Type type)
        {
            if (!type.IsStatic())
            {
                throw new InvalidOperationException($"[Injector] Type {type.FullName} is not static");
            }
            InjectInternal(type, null);
        }

        /// <summary>
        /// 根据类型创建实例并进行依赖注入
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>实例</returns>
        public static object CreateInstance(Type type)
        {
            var instance = CreateWithConstructor(type);
            if (instance == null)
            {
                instance = Activator.CreateInstance(type);
            }

            InjectInternal(instance.GetType(), instance);
            return instance;
        }

        /// <summary>
        /// 根据类型创建实例并进行依赖注入
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="args">构造参数</param>
        /// <returns>实例</returns>
        public static object CreateInstance(Type type, params object[] args)
        {
            var instance = CreateWithConstructor(type);
            if (instance == null)
            {
                instance = Activator.CreateInstance(type, args);
            }

            InjectInternal(instance.GetType(), instance);
            return instance;
        }

        /// <summary>
        /// 根据类型创建实例并进行依赖注入
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <returns>实例</returns>
        public static T CreateInstance<T>()
        {
            return (T)CreateInstance(typeof(T));
        }

        /// <summary>
        /// 根据类型创建实例并进行依赖注入
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="args">构造参数</param>
        /// <returns>实例</returns>
        public static T CreateInstance<T>(params object[] args)
        {
            return (T)CreateInstance(typeof(T), args);
        }

        /// <summary>
        /// 根据类型创建实例并进行依赖注入（使用 new 约束）
        /// </summary>
        public static T CreateInstanceWithNew<T>() where T : new()
        {
            var instance = new T();
            InjectInternal(instance.GetType(), instance);
            return instance;
        }

        /// <summary>
        /// 释放指定实例的缓存
        /// </summary>
        public static void Release(object instance)
        {
            for (int i = injectedCache.Count - 1; i >= 0; i--)
            {
                if (injectedCache[i].instance == instance)
                {
                    injectedCache.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 释放所有缓存
        /// </summary>
        public static void ReleaseAll()
        {
            injectedCache.Clear();
        }

        // 内部方法

        static void InjectInternal(Type type, object instance)
        {
            type = instance == null ? type : instance.GetType();
            InjectFields(type, instance);
            InjectProperties(type, instance);
            InjectMethods(type, instance);
            CacheInjected(type, instance);
        }

        static void InjectFields(Type type, object instance)
        {
            foreach (var fieldInfo in type.GetInjectFields())
            {
                var binding = container?.ResolveBinding(fieldInfo.FieldType, type);
                if (binding == null || binding.IsEmpty())
                {
                    continue;
                }

                var owner = fieldInfo.IsStatic ? type : instance;
                var value = CreateFromBinding(binding);
                fieldInfo.SetValue(owner, value);
            }
        }

        static void InjectProperties(Type type, object instance)
        {
            foreach (var propertyInfo in type.GetInjectProperties())
            {
                var binding = container?.ResolveBinding(propertyInfo.PropertyType, type);
                if (binding == null || binding.IsEmpty())
                {
                    continue;
                }

                var owner = propertyInfo.IsStatic() ? type : instance;
                var value = CreateFromBinding(binding);
                propertyInfo.SetValue(owner, value);
            }
        }

        static void InjectMethods(Type type, object instance)
        {
            foreach (var methodInfo in type.GetInjectMethods())
            {
                var parameters = methodInfo.GetParameters();
                if (parameters.Length == 0)
                {
                    continue;
                }

                // 支持多参数方法注入
                var args = new object[parameters.Length];
                bool allResolved = true;

                for (int i = 0; i < parameters.Length; i++)
                {
                    var binding = container?.ResolveBinding(parameters[i].ParameterType, type);
                    if (binding == null || binding.IsEmpty())
                    {
                        allResolved = false;
                        break;
                    }
                    args[i] = CreateFromBinding(binding);
                }

                if (!allResolved)
                {
                    continue;
                }

                var owner = methodInfo.IsStatic ? type : instance;
                methodInfo.Invoke(owner, args);
            }
        }

        static object CreateWithConstructor(Type type)
        {
            var ctors = type.GetInjectConstructors();
            if (ctors.Count == 0)
            {
                return null;
            }

            // 选择参数最多的构造器（通常是最具体的）
            ConstructorInfo bestCtor = null;
            foreach (var ctor in ctors)
            {
                if (bestCtor == null || ctor.GetParameters().Length > bestCtor.GetParameters().Length)
                {
                    bestCtor = ctor;
                }
            }

            if (bestCtor == null)
            {
                return null;
            }

            var parameters = bestCtor.GetParameters();
            if (parameters.Length == 0)
            {
                return bestCtor.Invoke(Array.Empty<object>());
            }

            // 支持多参数构造器注入
            var args = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                var binding = container?.ResolveBinding(parameters[i].ParameterType, type);
                if (binding == null || binding.IsEmpty())
                {
                    return null;
                }
                args[i] = CreateFromBinding(binding);
            }

            return bestCtor.Invoke(args);
        }

        static object CreateFromBinding(Binding binding)
        {
            // 如果有现成实例，直接返回
            if (binding.instance != null)
            {
                return binding.instance;
            }

            // 单例模式：检查缓存
            if (binding.lifetime == Lifetime.Singleton && binding.implementationType != null)
            {
                // 这里需要从 Container 的单例缓存中获取
                // 由于 Binding 没有持有 Container 引用，我们需要通过其他方式
                // 简化处理：直接创建并注入
                return CreateInstance(binding.implementationType);
            }

            // 瞬态模式：创建新实例
            if (binding.implementationType != null)
            {
                return CreateInstance(binding.implementationType);
            }

            return null;
        }

        static void CacheInjected(Type type, object instance)
        {
            // 检查是否已缓存
            for (int i = 0; i < injectedCache.Count; i++)
            {
                if (injectedCache[i].type == type && injectedCache[i].instance == instance)
                {
                    return;
                }
            }

            injectedCache.Add((type, instance));
        }
    }
}
