using System;

namespace FInject
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Property)]
    public class InjectAttribute : Attribute
    {
    }
}