using System;
using System.Linq;
using UnityEngine.Profiling;

namespace Framework.Editor
{
    public static partial class Utility
    {
        public static class Memory
        {
            static readonly Type objectType = typeof(UnityEngine.Object);

            /// <summary>
            /// 获取一个对象中是UnityEngine.Object的字段或属性运行时所占内存大小
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public static long GetUnityObjectFieldPropertyRuntimeMemorySize(object obj)
            {
                var type = obj.GetType();

                var size = 0L;

                var allFlag = Framework.Utility.Assembly.AllFlag;
                var field = type.GetFields(allFlag).FirstOrDefault(item => objectType.IsAssignableFrom(item.FieldType))?.GetValue(obj);
                size += field == null ? 0 : Profiler.GetRuntimeMemorySizeLong(field as UnityEngine.Object);

                var property = type.GetProperties(allFlag).FirstOrDefault(item => objectType.IsAssignableFrom(item.PropertyType))?.GetValue(obj);
                size += property == null ? 0 : Profiler.GetRuntimeMemorySizeLong(property as UnityEngine.Object);

                return size;
            }
        }
    }
}