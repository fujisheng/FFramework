using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public static partial class Utility
    {
        public static class String
        {
            static Dictionary<int, string> cache = new Dictionary<int, string>();

            [ThreadStatic]
            private static StringBuilder cachedStringBuilder = null;

            /// <summary>
            /// 获取格式化字符串。
            /// </summary>
            /// <param name="format">字符串格式。</param>
            /// <param name="arg0">字符串参数 0。</param>
            /// <returns>格式化后的字符串。</returns>
            public static string Format(string format, object arg0)
            {
                Assert.IfNull(format, new Exception("Format is invalid."));

                CheckCachedStringBuilder();
                cachedStringBuilder.Length = 0;
                cachedStringBuilder.AppendFormat(format, arg0);
                return cachedStringBuilder.ToString();
            }

            /// <summary>
            /// 获取格式化字符串。
            /// </summary>
            /// <param name="format">字符串格式。</param>
            /// <param name="arg0">字符串参数 0。</param>
            /// <param name="arg1">字符串参数 1。</param>
            /// <returns>格式化后的字符串。</returns>
            public static string Format(string format, object arg0, object arg1)
            {
                Assert.IfNull(format, new Exception("Format is invalid."));

                CheckCachedStringBuilder();
                cachedStringBuilder.Length = 0;
                cachedStringBuilder.AppendFormat(format, arg0, arg1);
                return cachedStringBuilder.ToString();
            }

            /// <summary>
            /// 获取格式化字符串。
            /// </summary>
            /// <param name="format">字符串格式。</param>
            /// <param name="arg0">字符串参数 0。</param>
            /// <param name="arg1">字符串参数 1。</param>
            /// <param name="arg2">字符串参数 2。</param>
            /// <returns>格式化后的字符串。</returns>
            public static string Format(string format, object arg0, object arg1, object arg2)
            {
                Assert.IfNull(format, new Exception("Format is invalid."));

                CheckCachedStringBuilder();
                cachedStringBuilder.Length = 0;
                cachedStringBuilder.AppendFormat(format, arg0, arg1, arg2);
                return cachedStringBuilder.ToString();
            }

            /// <summary>
            /// 获取格式化字符串。
            /// </summary>
            /// <param name="format">字符串格式。</param>
            /// <param name="args">字符串参数。</param>
            /// <returns>格式化后的字符串。</returns>
            public static string Format(string format, params object[] args)
            {
                Assert.IfNull(format, new Exception("Format is invalid."));
                Assert.IfNull(args, new Exception("Args is invalid."));

                CheckCachedStringBuilder();
                cachedStringBuilder.Length = 0;
                cachedStringBuilder.AppendFormat(format, args);
                return cachedStringBuilder.ToString();
            }

            private static void CheckCachedStringBuilder()
            {
                if (cachedStringBuilder == null)
                {
                    cachedStringBuilder = new StringBuilder(1024);
                }
            }

            /// <summary>
            /// 如果已经缓存了结果则直接返回否则才连接
            /// </summary>
            /// <param name="left"></param>
            /// <param name="right"></param>
            /// <returns></returns>
            public static string GetOrCombine(string left, string right)
            {
                var key = Hash.CombineHash(left.GetHashCode(), right.GetHashCode());
                bool get = cache.TryGetValue(key, out string result);
                if (get)
                {
                    return result;
                }

                result = Format("{0}{1}", left, right);
                cache.Add(key, result);
                return result;
            }
        }
    }
}
