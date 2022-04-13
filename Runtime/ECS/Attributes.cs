using System;

namespace Framework.ECS
{
    /// <summary>
    /// 在目标系统之前更新
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class UpdateBeforeAttribute : Attribute
    {
        public readonly Type target;

        public UpdateBeforeAttribute(Type target)
        {
            if (!typeof(ComponentSystem).IsAssignableFrom(target))
            {
                throw new Exception($"{target} is not {nameof(ComponentSystem)}");
            }

            this.target = target;
        }
    }

    /// <summary>
    /// 在目标系统之后更新
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class UpdateAfterAttribute : Attribute
    {
        public readonly Type target;

        public UpdateAfterAttribute(Type target)
        {
            if (!typeof(ComponentSystem).IsAssignableFrom(target))
            {
                throw new Exception($"{target} is not {nameof(ComponentSystem)}");
            }

            this.target = target;
        }
    }
}