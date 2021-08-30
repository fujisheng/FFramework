using System;

namespace Framework.Service
{
    /// <summary>
    /// 模块依赖的模块特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DependenciesAttribute : Attribute
    {
        public readonly Type[] dependencies;
        public DependenciesAttribute(params Type[] dependency)
        {
            this.dependencies = dependency;
        }
    }
}
