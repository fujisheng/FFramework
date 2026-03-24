using System;
using System.Reflection;

namespace Framework.IoC
{
    /// <summary>
    /// 类型工具扩展方法
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// 判断类型是否为静态类
        /// </summary>
        public static bool IsStatic(this Type type)
        {
            return type.IsAbstract && type.IsSealed;
        }

        /// <summary>
        /// 判断属性是否为静态
        /// </summary>
        public static bool IsStatic(this PropertyInfo propertyInfo)
        {
            var getMethod = propertyInfo.GetGetMethod();
            if (getMethod != null)
            {
                return getMethod.IsStatic;
            }

            var setMethod = propertyInfo.GetSetMethod();
            return setMethod != null && setMethod.IsStatic;
        }
    }
}
