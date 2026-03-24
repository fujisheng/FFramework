using System;

namespace FInject
{
    /// <summary>
    /// 依赖注入的绑定信息
    /// </summary>
    public class BindInfo : IDisposable
    {
        internal Type originType;
        internal Type bindType;
        internal Type containerType;
        internal object instance;
        internal Func<Type, bool> checker;

        internal BindInfo()
        {

        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="originType">要被注入的类型</param>
        internal BindInfo(Type originType)
        {
            this.originType = originType;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="originType">要被注入的类型</param>
        /// <param name="containerType">被注入所在的类</param>
        internal BindInfo(Type originType, Type containerType)
        {
            this.originType = originType;
            this.containerType = containerType;
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            originType = null;
            bindType = null;
            containerType = null;
            instance = null;
            checker = null;
        }

        /// <summary>
        /// 判断是否为空
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return bindType == null && instance == null;
        }

        /// <summary>
        /// 将某个类型绑定成T类型
        /// </summary>
        /// <typeparam name="T">注入后的类型</typeparam>
        /// <returns>绑定信息</returns>
        public BindInfo As<T>()
        {
            return As(typeof(T));
        }

        /// <summary>
        /// 将某个类型绑定成bindType类型
        /// </summary>
        /// <param name="bindType">注入后的类型</param>
        /// <returns>绑定信息</returns>
        public BindInfo As(Type bindType)
        {
            if (!originType.IsAssignableFrom(bindType))
            {
                throw new Exception($"{originType.FullName} is not assignable from {bindType.FullName}");
            }

            if (bindType.IsInterface)
            {
                throw new Exception($"{bindType} BindType can not be interface");
            }

            if (bindType.IsAbstract)
            {
                throw new Exception($"{bindType} BindType can not be abstract");
            }

            this.bindType = bindType;
            return this;
        }

        /// <summary>
        /// 指定只有这种类型才执行这种绑定方式
        /// </summary>
        /// <param name="type"></param>
        /// <returns>绑定信息</returns>
        public BindInfo When(Type type)
        {
            this.containerType = type;
            return this;
        }

        /// <summary>
        /// 指定只有这种类型下的才执行这种绑定方式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>绑定信息</returns>
        public BindInfo When<T>()
        {
            return When(typeof(T));
        }

        /// <summary>
        /// 用于判断某些条件下注入
        /// </summary>
        /// <param name="checker">检查是否满足条件</param>
        /// <returns>绑定信息</returns>
        public BindInfo Where(Func<Type, bool> checker)
        {
            if(checker == null)
            {
                throw new ArgumentNullException("checker");
            }
            this.checker = checker;

            return this;
        }

        /// <summary>
        /// 直接把某个类型注入成具体的实例
        /// </summary>
        /// <param name="instance">具体的实例</param>
        /// <returns>绑定信息</returns>
        public BindInfo AsInstance(object instance)
        {
            if(instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            if(originType == null)
            {
                throw new NullReferenceException("originType");
            }

            var instanceType = instance.GetType();
            if (!originType.IsAssignableFrom(instanceType))
            {
                throw new Exception($"{originType.FullName} is not assignable from {instanceType.FullName}");
            }

            this.instance = instance;
            return this;
        }

        /// <summary>
        /// 判断两个绑定信息是否一样
        /// </summary>
        /// <param name="other"></param>
        /// <returns>是否一样</returns>
        internal bool Equals(BindInfo other)
        {
            return originType == other.originType &&
                bindType == other.bindType &&
                containerType == other.containerType &&
                instance == other.instance &&
                checker == other.checker;
        }
    }
}