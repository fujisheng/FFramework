using System;

namespace Framework.IoC
{
    /// <summary>
    /// 标记依赖注入点的特性
    /// 可用于字段、属性、方法和构造器
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Property)]
    public class InjectAttribute : Attribute
    {
    }
}
