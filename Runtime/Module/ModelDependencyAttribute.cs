using System;

namespace Framework.Module
{
    /// <summary>
    /// 模块依赖的模块特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class Dependency : Attribute
    {
        public readonly Type[] dependency;
        public Dependency(params Type[] dependency)
        {
            this.dependency = dependency;
        }
    }
}
