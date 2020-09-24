using System;

namespace Framework.Module
{
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
