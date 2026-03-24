using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Module.Resource
{
    /// <summary>
    /// 资源泄漏检测器 - 仅在 Editor 或 Development Build 中启用
    /// 用于跟踪资源分配和释放，在应用退出时报告未释放的资源
    /// </summary>
    public static class ResourceLeakDetector
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        /// <summary>
        /// 采样率 (0.0-1.0)，1.0 = 100% 记录所有分配，0.1 = 10%
        /// </summary>
        public static float SampleRate { get; set; } = 0.1f;

        /// <summary>
        /// 分配信息
        /// </summary>
        private class AllocationInfo
        {
            public string StackTrace;
            public DateTime Timestamp;
            public int AllocationCount;
            public string AssetName;
        }

        private static readonly Dictionary<Framework.IReference, AllocationInfo> allocations =
            new Dictionary<Framework.IReference, AllocationInfo>();
        private static readonly System.Random random = new System.Random();
        private static readonly object lockObj = new object();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            Application.quitting += OnApplicationQuit;
        }

        /// <summary>
        /// 记录资源分配
        /// </summary>
        public static void RecordAllocation(Framework.IReference reference, string assetName = null)
        {
            if (random.NextDouble() > SampleRate)
            {
                return;
            }

            lock (lockObj)
            {
                if (!allocations.TryGetValue(reference, out var info))
                {
                    allocations[reference] = new AllocationInfo
                    {
                        StackTrace = Environment.StackTrace,
                        Timestamp = DateTime.Now,
                        AllocationCount = 1,
                        AssetName = assetName ?? reference?.ToString() ?? "Unknown"
                    };
                }
                else
                {
                    info.AllocationCount++;
                }
            }
        }

        /// <summary>
        /// 记录资源释放
        /// </summary>
        public static void RecordRelease(Framework.IReference reference)
        {
            lock (lockObj)
            {
                allocations.Remove(reference);
            }
        }

        private static void OnApplicationQuit()
        {
            ReportLeaks();
        }

        /// <summary>
        /// 报告未释放的资源
        /// </summary>
        public static void ReportLeaks()
        {
            lock (lockObj)
            {
                if (allocations.Count == 0)
                {
                    return;
                }

                Debug.LogWarning($"[ResourceLeakDetector] {allocations.Count} potential resource leaks detected:");

                int index = 1;
                foreach (var kvp in allocations)
                {
                    var info = kvp.Value;
                    Debug.LogWarning($"[{index}] Asset: {info.AssetName}");
                    Debug.LogWarning($"    Allocations: {info.AllocationCount}, Time: {info.Timestamp:HH:mm:ss}");

                    // 截断堆栈跟踪以避免日志过长
                    var stackTrace = info.StackTrace;
                    if (stackTrace.Length > 500)
                    {
                        stackTrace = stackTrace.Substring(0, 500) + "...";
                    }
                    Debug.LogWarning($"    StackTrace:\n{stackTrace}");
                    index++;
                }
            }
        }

        /// <summary>
        /// 获取当前未释放资源数量
        /// </summary>
        public static int GetUnreleasedCount()
        {
            lock (lockObj)
            {
                return allocations.Count;
            }
        }

        /// <summary>
        /// 清除所有记录
        /// </summary>
        public static void Clear()
        {
            lock (lockObj)
            {
                allocations.Clear();
            }
        }
#else
        // Release 构建中的空实现
        /// <summary>
        /// 采样率 (0.0-1.0)，1.0 = 100% 记录所有分配，0.1 = 10%
        /// </summary>
        public static float SampleRate { get; set; } = 0.0f;

        public static void RecordAllocation(Framework.IReference reference, string assetName = null) { }
        public static void RecordRelease(Framework.IReference reference) { }
        public static void ReportLeaks() { }
        public static int GetUnreleasedCount() { return 0; }
        public static void Clear() { }
#endif
    }
}
