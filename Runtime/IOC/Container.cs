using System;
using System.Collections.Generic;

namespace Framework.IoC
{
    /// <summary>
    /// Binding 优先级比较器，避免闭包分配
    /// </summary>
    internal class BindingPriorityComparer : IComparer<Binding>
    {
        private Type consumerType;

        public void SetConsumerType(Type type)
        {
            consumerType = type;
        }

        public int Compare(Binding x, Binding y)
        {
            // 预计算的优先级越高，排在前面
            if (x.priority != y.priority)
            {
                return y.priority.CompareTo(x.priority);
            }
            return 0;
        }
    }

    /// <summary>
    /// 依赖注入容器，保存注入的关系
    /// </summary>
    public class Container
    {
        // 契约类型 -> 绑定列表
        readonly Dictionary<Type, List<Binding>> bindingMap = new Dictionary<Type, List<Binding>>();
        
        // 单例缓存
        readonly Dictionary<Type, object> singletonCache = new Dictionary<Type, object>();
        
        // 复用比较器，避免每次分配
        readonly BindingPriorityComparer priorityComparer = new BindingPriorityComparer();

        /// <summary>
        /// 绑定契约类型（接口或基类）
        /// </summary>
        /// <typeparam name="TContract">契约类型</typeparam>
        /// <returns>绑定信息</returns>
        public Binding Bind<TContract>()
        {
            return Bind(typeof(TContract));
        }

        /// <summary>
        /// 绑定契约类型和实现类型（简洁绑定）
        /// </summary>
        /// <typeparam name="TContract">契约类型</typeparam>
        /// <typeparam name="TImplementation">实现类型</typeparam>
        /// <returns>绑定信息</returns>
        public Binding Bind<TContract, TImplementation>()
        {
            return Bind(typeof(TContract)).To<TImplementation>();
        }

        /// <summary>
        /// 绑定契约类型
        /// </summary>
        /// <param name="contractType">契约类型</param>
        /// <returns>绑定信息</returns>
        public Binding Bind(Type contractType)
        {
            if (!bindingMap.TryGetValue(contractType, out var bindings))
            {
                bindings = new List<Binding>();
                bindingMap[contractType] = bindings;
            }

            var binding = BindingPool.Pop();
            binding.contractType = contractType;
            
            // 检查是否已存在相同的绑定
            for (int i = 0; i < bindings.Count; i++)
            {
                if (bindings[i].Equals(binding))
                {
                    bindings[i] = binding;
                    return binding;
                }
            }

            bindings.Add(binding);
            return binding;
        }

        /// <summary>
        /// 获取最优绑定信息
        /// </summary>
        /// <param name="contractType">契约类型</param>
        /// <param name="consumerType">消费者类型</param>
        /// <returns>最优绑定信息，如果没有则返回 null</returns>
        internal Binding ResolveBinding(Type contractType, Type consumerType)
        {
            if (!bindingMap.TryGetValue(contractType, out var bindings))
            {
                return null;
            }

            if (bindings.Count == 0)
            {
                return null;
            }

            // 单一绑定直接返回
            if (bindings.Count == 1)
            {
                var single = bindings[0];
                single.CalculatePriority(consumerType);
                return single;
            }

            // 多绑定：预计算优先级然后排序
            for (int i = 0; i < bindings.Count; i++)
            {
                bindings[i].CalculatePriority(consumerType);
            }

            priorityComparer.SetConsumerType(consumerType);
            bindings.Sort(priorityComparer);

            return bindings[0];
        }
         
         
         
         
         
         
        
        /// <summary>
        /// 尝试解析一个实例
        /// </summary>
        /// <typeparam name="T">契约类型</typeparam>
        /// <param name="instance">解析出的实例</param>
        /// <returns>是否解析成功</returns>
        public bool TryResolve<T>(out T instance)
        {
            instance = default;
            var binding = ResolveBinding(typeof(T), null);
            if (binding == null || binding.IsEmpty())
            {
                return false;
            }

            var obj = CreateInstance(binding);
            if (obj is T typed)
            {
                instance = typed;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 解析一个实例
        /// </summary>
        /// <typeparam name="T">契约类型</typeparam>
        /// <returns>解析出的实例</returns>
        public T Resolve<T>()
        {
            if (TryResolve<T>(out var instance))
            {
                return instance;
            }
            throw new InvalidOperationException($"[IoC] No binding found for type {typeof(T).FullName}");
        }

        /// <summary>
        /// 创建绑定对应的实例
        /// </summary>
        object CreateInstance(Binding binding)
        {
            // 如果已有实例（FromInstance），直接返回
            if (binding.instance != null)
            {
                return binding.instance;
            }

            // 单例模式：检查缓存
            if (binding.lifetime == Lifetime.Singleton)
            {
                if (singletonCache.TryGetValue(binding.contractType, out var cached))
                {
                    return cached;
                }

                var instance = Injector.CreateInstance(binding.implementationType);
                singletonCache[binding.contractType] = instance;
                return instance;
            }

            // 瞬态模式：每次创建新实例
            return Injector.CreateInstance(binding.implementationType);
        }

        /// <summary>
        /// 检查是否已绑定指定类型
        /// </summary>
        public bool HasBinding<TContract>()
        {
            return HasBinding(typeof(TContract));
        }

        /// <summary>
        /// 检查是否已绑定指定类型
        /// </summary>
        public bool HasBinding(Type contractType)
        {
            return bindingMap.ContainsKey(contractType) && bindingMap[contractType].Count > 0;
        }

        /// <summary>
        /// 移除指定契约类型的所有绑定
        /// </summary>
        public void Unbind<TContract>()
        {
            Unbind(typeof(TContract));
        }

        /// <summary>
        /// 移除指定契约类型的所有绑定
        /// </summary>
        public void Unbind(Type contractType)
        {
            if (bindingMap.TryGetValue(contractType, out var bindings))
            {
                for (int i = 0; i < bindings.Count; i++)
                {
                    BindingPool.Push(bindings[i]);
                }
                bindings.Clear();
            }
            
            singletonCache.Remove(contractType);
        }

        /// <summary>
        /// 释放所有绑定
        /// </summary>
        public void Release()
        {
            foreach (var kv in bindingMap)
            {
                var bindings = kv.Value;
                foreach (var binding in bindings)
                {
                    BindingPool.Push(binding);
                }
            }

            bindingMap.Clear();
            singletonCache.Clear();
        }
    }
}
