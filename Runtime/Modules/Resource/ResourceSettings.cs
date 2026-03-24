using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Module.Resource
{
    /// <summary>
    /// 资源配置 ScriptableObject
    /// 用于运行时配置资源路径到 Bundle 名称的映射
    /// </summary>
    [CreateAssetMenu(fileName = "ResourceSettings", menuName = "Framework/Resource Settings")]
    public class ResourceSettings : ScriptableObject, IResourceMapping
    {
        [Serializable]
        public class ResourceMappingEntry
        {
            public string assetPath;
            public string bundleName;
        }

        [SerializeField]
        List<ResourceMappingEntry> mappings = new List<ResourceMappingEntry>();

        // 运行时缓存
        Dictionary<string, string> mappingCache;
        FolderBasedResourceMapping fallbackMapping;

        #region GC Configuration

        [Header("GC Configuration")]
        [SerializeField]
        [Tooltip("高内存压力时的最短GC间隔（秒）")]
        private float minGCInterval = 10f;

        [SerializeField]
        [Tooltip("低内存压力时的最长GC间隔（秒）")]
        private float maxGCInterval = 60f;

        [SerializeField]
        [Tooltip("高内存压力阈值（MB）")]
        private float highMemoryThresholdMB = 100f;

        [SerializeField]
        [Tooltip("低内存压力阈值（MB）")]
        private float lowMemoryThresholdMB = 50f;

        #endregion

        #region Logging Configuration

        [Header("Logging Configuration")]
        [SerializeField]
        [Tooltip("资源模块日志级别")]
        private LogLevel logLevel = LogLevel.Info;

        #endregion

        #region Concurrency Configuration

        [Header("Concurrency Configuration")]
        [SerializeField]
        [Tooltip("最大并发加载数")]
        private int maxConcurrentLoads = 3;

        #endregion

        #region Read-only Properties

        /// <summary>
        /// 高内存压力时的最短GC间隔（秒）
        /// </summary>
        public float MinGCInterval => minGCInterval;

        /// <summary>
        /// 低内存压力时的最长GC间隔（秒）
        /// </summary>
        public float MaxGCInterval => maxGCInterval;

        /// <summary>
        /// 高内存压力阈值（MB）
        /// </summary>
        public float HighMemoryThresholdMB => highMemoryThresholdMB;

        /// <summary>
        /// 低内存压力阈值（MB）
        /// </summary>
        public float LowMemoryThresholdMB => lowMemoryThresholdMB;

        /// <summary>
        /// 资源模块日志级别
        /// </summary>
        public LogLevel LogLevel => logLevel;

        /// <summary>
        /// 最大并发加载数
        /// </summary>
        public int MaxConcurrentLoads => maxConcurrentLoads;

        #endregion

        void OnEnable()
        {
            InitializeCache();
        }

        void InitializeCache()
        {
            mappingCache = new Dictionary<string, string>();
            foreach (var entry in mappings)
            {
                if (!string.IsNullOrEmpty(entry.assetPath) && !string.IsNullOrEmpty(entry.bundleName))
                {
                    var normalizedPath = NormalizePath(entry.assetPath);
                    mappingCache[normalizedPath] = entry.bundleName.ToLowerInvariant();
                }
            }
            fallbackMapping = new FolderBasedResourceMapping();
        }

        /// <summary>
        /// 获取资源对应的 Bundle 名称
        /// </summary>
        public string GetBundleName(string assetPath)
        {
            if (TryGetBundleName(assetPath, out var bundleName))
            {
                return bundleName;
            }
            return "default";
        }

        /// <summary>
        /// 尝试获取资源对应的 Bundle 名称
        /// </summary>
        public bool TryGetBundleName(string assetPath, out string bundleName)
        {
            if (mappingCache == null)
            {
                InitializeCache();
            }

            // 首先检查精确匹配
            var normalizedPath = NormalizePath(assetPath);
            if (mappingCache.TryGetValue(normalizedPath, out bundleName))
            {
                return true;
            }

            // 回退到路径推断
            return fallbackMapping.TryGetBundleName(assetPath, out bundleName);
        }

        /// <summary>
        /// 注册资源映射（运行时动态添加）
        /// </summary>
        public void RegisterMapping(string assetPath, string bundleName)
        {
            if (string.IsNullOrEmpty(assetPath) || string.IsNullOrEmpty(bundleName))
            {
                Debug.LogWarning("[ResourceSettings] Cannot register mapping with null or empty values");
                return;
            }

            var normalizedPath = NormalizePath(assetPath);
            mappingCache[normalizedPath] = bundleName.ToLowerInvariant();
            
            // 同时添加到序列化列表（仅在编辑器中持久化）
#if UNITY_EDITOR
            mappings.Add(new ResourceMappingEntry 
            { 
                assetPath = assetPath, 
                bundleName = bundleName 
            });
#endif
        }

        /// <summary>
        /// 移除资源映射
        /// </summary>
        public bool RemoveMapping(string assetPath)
        {
            var normalizedPath = NormalizePath(assetPath);
            return mappingCache.Remove(normalizedPath);
        }

        /// <summary>
        /// 获取所有已注册的资源路径
        /// </summary>
        public IEnumerable<string> GetAllAssetPaths()
        {
            if (mappingCache == null)
            {
                return new List<string>();
            }
            return mappingCache.Keys;
        }

        /// <summary>
        /// 清除所有映射
        /// </summary>
        public void Clear()
        {
            mappingCache?.Clear();
        }

        /// <summary>
        /// 获取映射数量
        /// </summary>
        public int MappingCount => mappingCache?.Count ?? 0;

        /// <summary>
        /// 单例访问
        /// </summary>
        static ResourceSettings instance;
        public static ResourceSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<ResourceSettings>("ResourceSettings");
                    if (instance == null)
                    {
                        Debug.LogWarning("[ResourceSettings] No ResourceSettings.asset found in Resources folder. " +
                            "Please create one via Tools > Framework > Create Resource Settings");
                        // 创建空实例作为兜底
                        instance = CreateInstance<ResourceSettings>();
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 规范化路径（统一使用小写和正斜杠）
        /// </summary>
        string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return path;
            }
            return path.Replace('\\', '/').ToLowerInvariant();
        }

        /// <summary>
        /// 重置单例（用于测试）
        /// </summary>
        public static void ResetInstance()
        {
            instance = null;
        }
    }
}
