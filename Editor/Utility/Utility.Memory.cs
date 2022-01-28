using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Profiling;

namespace Framework.Editor
{
    public static partial class Utility
    {
        public static class Memory
        {
            static readonly Type objectType = typeof(UnityEngine.Object);
            static readonly BindingFlags allFlag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;

            /// <summary>
            /// 获取一个对象中是UnityEngine.Object的字段或属性运行时所占内存大小
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public static long GetUnityObjectFieldPropertyRuntimeMemorySize(object obj)
            {
                var type = obj.GetType();

                var size = 0L;

                var field = type.GetFields(allFlag).FirstOrDefault(item => objectType.IsAssignableFrom(item.FieldType))?.GetValue(obj);
                size += field == null ? 0 : Profiler.GetRuntimeMemorySizeLong(field as UnityEngine.Object);

                var property = type.GetProperties(allFlag).FirstOrDefault(item => objectType.IsAssignableFrom(item.PropertyType))?.GetValue(obj);
                size += property == null ? 0 : Profiler.GetRuntimeMemorySizeLong(property as UnityEngine.Object);

                return size;
            }
        }
    }
}