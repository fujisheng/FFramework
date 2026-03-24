using System;
using UnityEngine;

namespace Framework.Module.Resource
{
    /// <summary>
    /// 日志级别
    /// </summary>
    public enum LogLevel
    {
        Verbose = 0,
        Debug = 1,
        Info = 2,
        Warning = 3,
        Error = 4
    }

    /// <summary>
    /// 资源模块日志工具
    /// 支持运行时日志级别控制
    /// </summary>
    public static class ResourceLogger
    {
        /// <summary>
        /// 当前日志级别，从 ResourceSettings 获取默认值
        /// </summary>
        public static LogLevel CurrentLevel
        {
            get => currentLevel;
            set => currentLevel = value;
        }
        static LogLevel currentLevel = ResourceSettings.Instance.LogLevel;

        /// <summary>
        /// 检查指定级别是否允许输出
        /// </summary>
        static bool IsLevelEnabled(LogLevel level)
        {
            return level >= CurrentLevel;
        }

        /// <summary>
        /// 格式化日志消息
        /// </summary>
        static string FormatMessage(string category, string message)
        {
            return $"[{category}] {DateTime.Now:HH:mm:ss.fff} {message}";
        }

        /// <summary>
        /// 输出 Verbose 级别日志
        /// </summary>
        public static void Verbose(string category, string message)
        {
            if (IsLevelEnabled(LogLevel.Verbose))
            {
                UnityEngine.Debug.Log(FormatMessage(category, message));
            }
        }

        /// <summary>
        /// 输出 Debug 级别日志
        /// </summary>
        public static void Debug(string category, string message)
        {
            if (IsLevelEnabled(LogLevel.Debug))
            {
                UnityEngine.Debug.Log(FormatMessage(category, message));
            }
        }

        /// <summary>
        /// 输出 Info 级别日志
        /// </summary>
        public static void Info(string category, string message)
        {
            if (IsLevelEnabled(LogLevel.Info))
            {
                UnityEngine.Debug.Log(FormatMessage(category, message));
            }
        }

        /// <summary>
        /// 输出 Warning 级别日志
        /// </summary>
        public static void Warning(string category, string message)
        {
            if (IsLevelEnabled(LogLevel.Warning))
            {
                UnityEngine.Debug.LogWarning(FormatMessage(category, message));
            }
        }

        /// <summary>
        /// 输出 Error 级别日志
        /// </summary>
        public static void Error(string category, string message)
        {
            if (IsLevelEnabled(LogLevel.Error))
            {
                UnityEngine.Debug.LogError(FormatMessage(category, message));
            }
        }

        #region 向后兼容的旧方法（使用 Conditional 特性）

        [System.Diagnostics.Conditional("DEBUG_RESOURCE")]
        public static void Log(string message)
        {
            UnityEngine.Debug.Log($"[Resource] {DateTime.Now:HH:mm:ss.fff} {message}");
        }

        [System.Diagnostics.Conditional("DEBUG_RESOURCE")]
        public static void LogLoad(string assetName)
        {
            Log($"Load: {assetName}");
        }

        [System.Diagnostics.Conditional("DEBUG_RESOURCE")]
        public static void LogLoadAsync(string assetName)
        {
            Log($"LoadAsync: {assetName}");
        }

        [System.Diagnostics.Conditional("DEBUG_RESOURCE")]
        public static void LogRelease(string assetName)
        {
            Log($"Release: {assetName}");
        }

        [System.Diagnostics.Conditional("DEBUG_RESOURCE")]
        public static void LogCollect(int nodeCount)
        {
            Log($"Collect: {nodeCount} nodes");
        }

        [System.Diagnostics.Conditional("DEBUG_RESOURCE")]
        public static void LogDelete(int whiteCount)
        {
            Log($"Delete: {whiteCount} nodes to release");
        }

        [System.Diagnostics.Conditional("DEBUG_RESOURCE")]
        public static void LogInstantiate(string assetName)
        {
            Log($"Instantiate: {assetName}");
        }

        [System.Diagnostics.Conditional("DEBUG_RESOURCE")]
        public static void LogDestroy(string assetName)
        {
            Log($"Destroy: {assetName}");
        }

        [System.Diagnostics.Conditional("DEBUG_RESOURCE")]
        public static void LogBundleLoad(string bundleName)
        {
            Log($"BundleLoad: {bundleName}");
        }

        [System.Diagnostics.Conditional("DEBUG_RESOURCE")]
        public static void LogAlloc(string owner, string target)
        {
            Log($"Alloc: {owner} -> {target}");
        }

        [System.Diagnostics.Conditional("DEBUG_RESOURCE")]
        public static void LogWarning(string message)
        {
            UnityEngine.Debug.LogWarning($"[Resource] {message}");
        }

        [System.Diagnostics.Conditional("DEBUG_RESOURCE")]
        public static void LogError(string message)
        {
            UnityEngine.Debug.LogError($"[Resource] {message}");
        }

        #endregion
    }
}
