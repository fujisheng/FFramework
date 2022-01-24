using System;

namespace Framework
{
    public static partial class Utility
    {
        /// <summary>
        /// 断言实用工具类
        /// </summary>
        public static class Assert
        {
            /// <summary>
            /// 如果为空抛出异常
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="obj">object</param>
            /// <param name="exception">异常</param>
            public static void IfNull<T>(T obj, Exception exception) where T : class
            {
                if (obj == null)
                {
                    throw exception;
                }
            }

            /// <summary>
            /// 如果是某种类型抛出异常
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="obj"></param>
            /// <param name="exception"></param>
            public static void IfIs<T>(object obj, Exception exception)
            {
                if (obj is T)
                {
                    throw exception;
                }
            }

            /// <summary>
            /// 如果是某种类型 则抛出异常
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="type">类型</param>
            /// <param name="exception">异常</param>
            public static void IfIs<T>(Type type, Exception exception)
            {
                if (typeof(T).IsAssignableFrom(type))
                {
                    throw exception;
                }
            }

            /// <summary>
            /// 如果不是某种类型抛出异常
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="obj"></param>
            /// <param name="exception"></param>
            public static void IfNot<T>(object obj, Exception exception)
            {
                if (!(obj is T))
                {
                    throw exception;
                }
            }

            /// <summary>
            /// 如果不是某种类型 则抛出异常
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="type">类型</param>
            /// <param name="exception">异常</param>
            public static void IfNot<T>(Type type, Exception exception)
            {
                if (!typeof(T).IsAssignableFrom(type))
                {
                    throw exception;
                }
            }

            /// <summary>
            /// 如果为true则抛出异常
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="exception"></param>
            public static void IfTrue(bool obj, Exception exception)
            {
                if(obj == true)
                {
                    throw exception;
                }
            }

            /// <summary>
            /// 如果为false抛出异常
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="exception">异常</param>
            public static void IfFalse(bool obj, Exception exception)
            {
                if (obj == false)
                {
                    throw exception;
                }
            }

            /// <summary>
            /// 如果为空或者null
            /// </summary>
            /// <param name="str">字符串</param>
            /// <param name="exception">异常</param>
            public static void IfIsNullOrEmpty(string str, Exception exception)
            {
                if (string.IsNullOrEmpty(str))
                {
                    throw exception;
                }
            }

            #region Dictionary
            /// <summary>
            /// 字典不包含某个key则抛出异常
            /// </summary>
            /// <typeparam name="TKey"></typeparam>
            /// <typeparam name="TValue"></typeparam>
            /// <param name="dic">字典</param>
            /// <param name="key">key</param>
            /// <param name="exception">异常</param>
            public static void IfNotContainsKey<TKey, TValue>(System.Collections.Generic.Dictionary<TKey, TValue> dic, TKey key, Exception exception)
            {
                if (!dic.ContainsKey(key))
                {
                    throw exception;
                }
            }

            /// <summary>
            /// 如果字段包含某个Key怎抛出异常
            /// </summary>
            /// <typeparam name="TKey"></typeparam>
            /// <typeparam name="TValue"></typeparam>
            /// <param name="dic">字典</param>
            /// <param name="key">key</param>
            /// <param name="exception">异常</param>
            public static void IfContainsKey<TKey, TValue>(System.Collections.Generic.Dictionary<TKey, TValue> dic, TKey key, Exception exception)
            {
                if (dic.ContainsKey(key))
                {
                    throw exception;
                }
            }
            #endregion
        }
    }
}
