using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Framework.Module.Resource
{
    /// <summary>
    /// 基于文件夹的资源映射实现
    /// 规则：一个文件夹对应一个Bundle
    /// 如果文件夹下只有子文件夹没有文件，则向上查找
    /// </summary>
    public class FolderBasedResourceMapping : IResourceMapping
    {
        readonly Dictionary<string, string> mapping;
        readonly string defaultBundleName;

        public FolderBasedResourceMapping(string defaultBundle = "default")
        {
            mapping = new Dictionary<string, string>();
            defaultBundleName = defaultBundle;
        }

        /// <summary>
        /// 根据资源路径获取Bundle名称
        /// 规则：Assets/Data/Prefabs/HomeView.prefab -> prefabs
        /// </summary>
        public string GetBundleName(string assetPath)
        {
            if (TryGetBundleName(assetPath, out var bundleName))
            {
                return bundleName;
            }
            return defaultBundleName;
        }

        public bool TryGetBundleName(string assetPath, out string bundleName)
        {
            // 首先检查是否有精确匹配
            if (mapping.TryGetValue(NormalizePath(assetPath), out bundleName))
            {
                return true;
            }

            // 根据路径规则推断
            bundleName = InferBundleNameFromPath(assetPath);
            return bundleName != null;
        }

        /// <summary>
        /// 从资源路径推断Bundle名称
        /// </summary>
        string InferBundleNameFromPath(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return null;
            }

            // 规范化路径
            var path = NormalizePath(assetPath);
            
            // 移除 "Assets/" 前缀
            if (path.StartsWith("assets/"))
            {
                path = path.Substring(7);
            }

            // 获取目录部分
            var directory = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(directory))
            {
                return defaultBundleName;
            }

            // 获取最后一级目录名作为Bundle名
            var parts = directory.Split('/');
            if (parts.Length > 0)
            {
                var lastFolder = parts[parts.Length - 1];
                if (!string.IsNullOrEmpty(lastFolder))
                {
                    return lastFolder.ToLowerInvariant();
                }
            }

            return defaultBundleName;
        }

        public void RegisterMapping(string assetPath, string bundleName)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                throw new ArgumentException("Asset path cannot be null or empty", nameof(assetPath));
            }

            if (string.IsNullOrEmpty(bundleName))
            {
                throw new ArgumentException("Bundle name cannot be null or empty", nameof(bundleName));
            }

            mapping[NormalizePath(assetPath)] = bundleName.ToLowerInvariant();
        }

        public bool RemoveMapping(string assetPath)
        {
            return mapping.Remove(NormalizePath(assetPath));
        }

        public IEnumerable<string> GetAllAssetPaths()
        {
            return mapping.Keys;
        }

        public void Clear()
        {
            mapping.Clear();
        }

        /// <summary>
        /// 批量注册映射（从配置）
        /// </summary>
        public void RegisterMappings(IEnumerable<KeyValuePair<string, string>> mappings)
        {
            foreach (var kvp in mappings)
            {
                RegisterMapping(kvp.Key, kvp.Value);
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
    }
}
