using System;

namespace Framework.Service
{
    /// <summary>
    /// 模块依赖的模块特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    internal class DependenciesAttribute : Attribute
    {
        public readonly Type[] dependencies;
        public DependenciesAttribute(params Type[] dependency)
        {
            this.dependencies = dependency;
        }
    }

    /// <summary>
    /// 标记某个模块是所有模块的默认依赖
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    internal class DefaultServiceAttribute : Attribute { }
}
