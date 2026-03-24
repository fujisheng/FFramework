using System;

namespace Framework.IoC
{
    /// <summary>
    /// 生命周期类型
    /// </summary>
    public enum Lifetime
    {
        /// <summary>
        /// 每次请求都创建新实例
        /// </summary>
        Transient,
        
        /// <summary>
        /// 单例模式，全局只有一个实例
        /// </summary>
        Singleton
    }

    /// <summary>
    /// 依赖注入的绑定信息
    /// 使用行业标准的命名约定
    /// </summary>
    public class Binding : IDisposable
    {
        // 使用行业标准命名
        internal Type contractType;        // 契约类型（要被注入的类型）
        internal Type implementationType;  // 实现类型
        internal Type targetType;          // 目标类型（被注入所在的类）
        internal object instance;          // 实例
        internal Func<Type, bool> condition; // 条件检查器
        internal Lifetime lifetime = Lifetime.Transient; // 生命周期
        internal int priority;             // 预计算的优先级

        internal Binding()
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="contractType">契约类型（要被注入的类型）</param>
        internal Binding(Type contractType)
        {
            this.contractType = contractType;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="contractType">契约类型（要被注入的类型）</param>
        /// <param name="targetType">目标类型（被注入所在的类）</param>
        internal Binding(Type contractType, Type targetType)
        {
            this.contractType = contractType;
            this.targetType = targetType;
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            contractType = null;
            implementationType = null;
            targetType = null;
            instance = null;
            condition = null;
            lifetime = Lifetime.Transient;
            priority = 0;
        }

        /// <summary>
        /// 判断是否为空
        /// </summary>
        public bool IsEmpty()
        {
            return implementationType == null && instance == null;
        }

        /// <summary>
        /// 计算优先级（避免每次排序时重新计算）
        /// </summary>
        internal void CalculatePriority(Type consumerType)
        {
            priority = 0;
            
            // 目标类型匹配优先级最高
            if (targetType == consumerType)
                priority += 100;
            
            // 条件检查器匹配次之
            if (condition != null && condition(consumerType))
                priority += 50;
            
            // 有实例再次之
            if (instance != null)
                priority += 25;
        }

        /// <summary>
        /// 将契约类型绑定到实现类型
        /// </summary>
        /// <typeparam name="TImplementation">实现类型</typeparam>
        /// <returns>绑定信息</returns>
        public Binding To<TImplementation>()
        {
            return To(typeof(TImplementation));
        }

        /// <summary>
        /// 将契约类型绑定到实现类型
        /// </summary>
        /// <param name="implementationType">实现类型</param>
        /// <returns>绑定信息</returns>
        public Binding To(Type implementationType)
        {
            if (!contractType.IsAssignableFrom(implementationType))
            {
                throw new InvalidOperationException(
                    $"[IoC] {contractType.FullName} is not assignable from {implementationType.FullName}");
            }

            if (implementationType.IsInterface)
            {
                throw new InvalidOperationException(
                    $"[IoC] Implementation type {implementationType} cannot be an interface");
            }

            if (implementationType.IsAbstract)
            {
                throw new InvalidOperationException(
                    $"[IoC] Implementation type {implementationType} cannot be abstract");
            }

            this.implementationType = implementationType;
            return this;
        }

        /// <summary>
        /// 指定只有特定目标类型才执行此绑定
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <returns>绑定信息</returns>
        public Binding WhenInjectedInto(Type type)
        {
            this.targetType = type;
            return this;
        }

        /// <summary>
        /// 指定只有特定目标类型才执行此绑定
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>绑定信息</returns>
        public Binding WhenInjectedInto<T>()
        {
            return WhenInjectedInto(typeof(T));
        }

        /// <summary>
        /// 用于判断某些条件下注入
        /// </summary>
        /// <param name="condition">条件检查器</param>
        /// <returns>绑定信息</returns>
        public Binding When(Func<Type, bool> condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }
            this.condition = condition;
            return this;
        }

        /// <summary>
        /// 直接将契约类型绑定到具体实例
        /// </summary>
        /// <param name="instance">具体的实例</param>
        /// <returns>绑定信息</returns>
        public Binding FromInstance(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (contractType == null)
            {
                throw new NullReferenceException("[IoC] contractType is null");
            }

            var instanceType = instance.GetType();
            if (!contractType.IsAssignableFrom(instanceType))
            {
                throw new InvalidOperationException(
                    $"[IoC] {contractType.FullName} is not assignable from {instanceType.FullName}");
            }

            this.instance = instance;
            this.lifetime = Lifetime.Singleton;
            return this;
        }

        /// <summary>
        /// 设置为单例模式
        /// </summary>
        /// <returns>绑定信息</returns>
        public Binding AsSingle()
        {
            this.lifetime = Lifetime.Singleton;
            return this;
        }

        /// <summary>
        /// 设置为瞬态模式（每次创建新实例）
        /// </summary>
        /// <returns>绑定信息</returns>
        public Binding AsTransient()
        {
            this.lifetime = Lifetime.Transient;
            return this;
        }

        /// <summary>
        /// 判断两个绑定信息是否一样
        /// </summary>
        internal bool Equals(Binding other)
        {
            return contractType == other.contractType &&
                implementationType == other.implementationType &&
                targetType == other.targetType &&
                instance == other.instance &&
                condition == other.condition;
        }
    }
}
