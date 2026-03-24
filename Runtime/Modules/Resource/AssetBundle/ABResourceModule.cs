using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Framework.Module.Resource
{
    internal class ABResourceModule : Module
    {
        float lastCollectTime = 0f;
        float currentGCInterval = 20f;
        ConcurrentDictionary<string, AssetLoader> loaderMapping = new ConcurrentDictionary<string, AssetLoader>();

        /// <summary>
        /// 同步加载指定组中的所有资源
        /// </summary>
        public List<T> LoadAll<T>(string group) where T : Object
        {
            if (string.IsNullOrEmpty(group))
            {
                Debug.LogWarning("LoadAll: group name is null or empty");
                return new List<T>();
            }

            // 按需加载组 bundle
            var bundle = BundleLoader.Instance.Load(group);
            if (bundle == null)
            {
                Debug.LogWarning($"LoadAll: failed to load bundle for group '{group}'");
                return new List<T>();
            }

            var assetNames = bundle.GetAllAssetNames();
            var results = new List<T>();

            foreach (var name in assetNames)
            {
                var asset = bundle.LoadAsset(name);
                if (asset != null && asset.Is<T>())
                {
                    results.Add(asset.As<T>());
                }
            }

            return results;
        }

        /// <summary>
        /// 异步加载指定组中的所有资源
        /// </summary>
        public async ValueTask<List<T>> LoadAllAsync<T>(string group, CancellationToken cancellationToken = default) where T : Object
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(group))
            {
                Debug.LogWarning("LoadAllAsync: group name is null or empty");
                return new List<T>();
            }

            // 按需异步加载组 bundle
            var bundle = await BundleLoader.Instance.LoadAsync(group, 0, cancellationToken);
            if (bundle == null)
            {
                Debug.LogWarning($"LoadAllAsync: failed to load bundle for group '{group}'");
                return new List<T>();
            }

            var assetNames = bundle.GetAllAssetNames();
            var results = new List<T>();

            foreach (var name in assetNames)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var asset = await bundle.LoadAssetAsync(name);
                if (asset != null && asset.Is<T>())
                {
                    results.Add(asset.As<T>());
                }
            }

            return results;
        }

        // GC 配置从 ResourceSettings 获取
        float MinGCInterval => ResourceSettings.Instance.MinGCInterval;
        float MaxGCInterval => ResourceSettings.Instance.MaxGCInterval;
        float HighMemoryThresholdMB => ResourceSettings.Instance.HighMemoryThresholdMB;
        float LowMemoryThresholdMB => ResourceSettings.Instance.LowMemoryThresholdMB;
        const float DefaultGCInterval = 20f;  // 默认间隔

        public T Load<T>(string name) where T : Object
        {
            var group = GetGroup(name);
            var loader = GetOrCreateLoader(group);
            return loader.Load<T>(name);
        }

        public T Load<T>(string groupName, string name) where T : Object
        {
            var loader = GetOrCreateLoader(groupName);
            return loader.Load<T>(name);
        }

        public async ValueTask<T> LoadAsync<T>(string name, CancellationToken cancellationToken = default) where T : Object
        {
            cancellationToken.ThrowIfCancellationRequested();
            var group = GetGroup(name);
            var loader = GetOrCreateLoader(group);
            return await loader.LoadAsync<T>(name, cancellationToken);
        }

        public async ValueTask<T> LoadAsync<T>(string groupName, string assetName, CancellationToken cancellationToken = default) where T : Object
        {
            cancellationToken.ThrowIfCancellationRequested();
            var loader = GetOrCreateLoader(groupName);
            return await loader.LoadAsync<T>(assetName, cancellationToken);
        }

        public GameObject Instantiate(string name)
        {
            var group = GetGroup(name);
            var loader = GetOrCreateLoader(group);
            return loader.Instantiate(name);
        }

        public async ValueTask<GameObject> InstantiateAsync(string name)
        {
            var group = GetGroup(name);
            var loader = GetOrCreateLoader(group);
            return await loader.InstantiateAsync(name);
        }

        string GetGroup(string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                return "default";
            }

            // 从路径推断组名
            // 规则：Assets/Data/Prefabs/X.prefab -> prefabs
            var path = assetName.Replace('\\', '/').ToLowerInvariant();
            
            if (path.StartsWith("assets/"))
            {
                path = path.Substring(7);
            }

            var lastSlash = path.LastIndexOf('/');
            if (lastSlash > 0)
            {
                var dir = path.Substring(0, lastSlash);
                var dirSlash = dir.LastIndexOf('/');
                if (dirSlash >= 0)
                {
                    return dir.Substring(dirSlash + 1);
                }
                return dir;
            }

            return "default";
        }

        AssetLoader GetOrCreateLoader(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                UnityEngine.Debug.LogWarning($"ResourceGroup is empty, default assetLoader will be used!!!");
            }

            // 使用GetOrAdd确保并发安全
            return loaderMapping.GetOrAdd(groupName, name => new AssetLoader());
        }

        public void ReleaseAsset(string assetName)
        {
            var group = GetGroup(assetName);
            if(!loaderMapping.TryGetValue(group, out var loader))
            {
                return;
            }
            loader.Release(assetName);
        }

        public void ReleaseGroup(string groupName)
        {
            if(!loaderMapping.TryGetValue(groupName, out var loader))
            {
                return;
            }
            loader.Release();
        }

        internal override void OnLateUpdate()
        {
            if (Time.realtimeSinceStartup - lastCollectTime > currentGCInterval)
            {
                // 执行 GC
                AssetsReferenceTree.Instance.Collect();
                AssetsReferenceTree.Instance.Delete();
                lastCollectTime = Time.realtimeSinceStartup;

                // 动态调整下一次 GC 间隔（仅在 GC 触发时检查内存压力，避免频繁调用 Profiler API）
                currentGCInterval = CalculateDynamicGCInterval();
            }
        }

        /// <summary>
        /// 根据当前内存压力计算动态 GC 间隔
        /// 高内存压力 -> 短间隔（更频繁 GC）
        /// 低内存压力 -> 长间隔（减少 GC 频率）
        /// </summary>
        float CalculateDynamicGCInterval()
        {
            // 获取当前已分配内存（字节）
            long allocatedBytes = Profiler.GetTotalAllocatedMemoryLong();
            float allocatedMB = allocatedBytes / (1024f * 1024f);

            float newInterval;

            if (allocatedMB >= HighMemoryThresholdMB)
            {
                // 高内存压力：使用最短间隔
                newInterval = MinGCInterval;
            }
            else if (allocatedMB <= LowMemoryThresholdMB)
            {
                // 低内存压力：使用最长间隔
                newInterval = MaxGCInterval;
            }
            else
            {
                // 中等内存压力：线性插值计算间隔
                // 在 LowMemoryThresholdMB 和 HighMemoryThresholdMB 之间线性变化
                float t = (allocatedMB - LowMemoryThresholdMB) / (HighMemoryThresholdMB - LowMemoryThresholdMB);
                newInterval = Mathf.Lerp(MaxGCInterval, MinGCInterval, t);
            }

#if DEBUG_RESOURCE
            Debug.Log($"[ABResourceModule] GC interval adjusted: {newInterval:F1}s (memory: {allocatedMB:F1}MB)");
#endif

            return newInterval;
        }
    }
}
