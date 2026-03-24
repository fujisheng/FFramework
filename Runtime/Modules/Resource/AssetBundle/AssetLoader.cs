using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Threading.Tasks;

namespace Framework.Module.Resource
{
    public class AssetLoader : IReference
    {
        // 使用 ResourceSettings 替代硬编码映射
        IResourceMapping mapping;
        
        IResourceMapping GetMapping()
        {
            if (mapping == null)
            {
                mapping = ResourceSettings.Instance;
            }
            return mapping;
        }

        // 使用Lazy<T>实现并发安全加载，确保同一资源只加载一次
        readonly ConcurrentDictionary<string, Lazy<Asset>> loadedAsset;
        readonly ConcurrentDictionary<GameObject, Asset> instantiatedGameObject;

        public AssetLoader()
        {
            loadedAsset = new ConcurrentDictionary<string, Lazy<Asset>>();
            instantiatedGameObject = new ConcurrentDictionary<GameObject, Asset>();
        }

        Asset LoadInternal(string name)
        {
            if (!GetMapping().TryGetBundleName(name, out var bundleName))
            {
                ResourceLogger.Verbose("AssetLoader", $"Load failed: {name} not found in mapping");
                return null;
            }

            ResourceLogger.Verbose("AssetLoader", $"Load: {name} from bundle {bundleName}");
            var bundle = BundleLoader.Instance.Load(bundleName);
            var asset = bundle.LoadAsset(name);
            if (asset != null)
            {
                AssetsReferenceTree.Instance.Alloc(asset, this);
                ResourceLeakDetector.RecordAllocation(asset, name);
            }
            return asset;
        }

        Asset Load(string name)
        {
            // 使用GetOrAdd + Lazy模式确保并发安全
            var lazyAsset = loadedAsset.GetOrAdd(name, n =>
                new Lazy<Asset>(() => LoadInternal(n), System.Threading.LazyThreadSafetyMode.ExecutionAndPublication));
            
            try
            {
                return lazyAsset.Value;
            }
            catch
            {
                // 加载失败时移除缓存，允许重试
                loadedAsset.TryRemove(name, out _);
                throw;
            }
        }

        async Task<Asset> LoadInternalAsync(string name, CancellationToken cancellationToken)
        {
            const int maxRetries = 3;
            Exception lastException = null;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    return await TryLoadInternalAsync(name, cancellationToken);
                }
                catch (Exception ex) when (IsTransientError(ex) && attempt < maxRetries)
                {
                    lastException = ex;
                    var delay = 0.5f * attempt;
                    ResourceLogger.Warning("AssetLoader", $"Load failed, retry {attempt}/{maxRetries - 1}: {name}");
                    await Task.Delay(TimeSpan.FromSeconds(delay), cancellationToken);
                }
            }

            // 所有重试都失败，抛出详细异常
            throw new Exception($"Load failed after {maxRetries} retries: {name}", lastException);
        }

        async Task<Asset> TryLoadInternalAsync(string name, CancellationToken cancellationToken)
        {
            if (!GetMapping().TryGetBundleName(name, out var bundleName))
            {
                ResourceLogger.Verbose("AssetLoader", $"LoadAsync failed: {name} not found in mapping");
                return null;
            }

            ResourceLogger.Verbose("AssetLoader", $"LoadAsync: {name} from bundle {bundleName}");
            cancellationToken.ThrowIfCancellationRequested();
            var bundle = await BundleLoader.Instance.LoadAsync(bundleName, 0, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            var asset = await bundle.LoadAssetAsync(name);
            if (asset != null)
            {
                AssetsReferenceTree.Instance.Alloc(asset, this);
            }
            return asset;
        }

        static bool IsTransientError(Exception ex)
        {
            // 瞬态错误：网络、IO相关异常
            if (ex is IOException)
            {
                return true;
            }

            // 检查内部异常
            if (ex.InnerException != null)
            {
                return IsTransientError(ex.InnerException);
            }

            return false;
        }

        async ValueTask<Asset> LoadAsync(string name, CancellationToken cancellationToken = default)
        {
            // 检查是否已缓存
            if (loadedAsset.TryGetValue(name, out var existingLazy))
            {
                return existingLazy.Value;
            }

            // 异步加载
            var asset = await LoadInternalAsync(name, cancellationToken);
            if (asset == null)
            {
                return null;
            }

            // 使用Lazy包装并缓存
            var lazyAsset = new Lazy<Asset>(() => asset);
            loadedAsset.TryAdd(name, lazyAsset);
            
            return asset;
        }

        public T Load<T>(string name) where T : Object
        {
            var asset = Load(name);
            if (asset == null)
            {
                return null;
            }
            return asset.As<T>();
        }

        public async ValueTask<T> LoadAsync<T>(string name, CancellationToken cancellationToken = default) where T : Object
        {
            var asset = await LoadAsync(name, cancellationToken);
            if (asset == null)
            {
                return null;
            }
            return asset.As<T>();
        }

        public void Release(Object obj, bool force = false)
        {
            if (obj == null)
            {
                return;
            }

            if (!loadedAsset.TryGetValue(obj.name, out var lazyAsset))
            {
                ResourceLogger.Verbose("AssetLoader", $"Release(Object) failed: {obj.name} not found in loaded assets");
                return;
            }

            loadedAsset.TryRemove(obj.name, out _);
            ResourceLogger.Verbose("AssetLoader", $"Release(Object): {obj.name} (force={force})");
            AssetsReferenceTree.Instance.Release(lazyAsset.Value, force);
        }

        public void Release(string assetName, bool force = false)
        {
            if(!loadedAsset.TryGetValue(assetName, out var lazyAsset))
            {
                ResourceLogger.Verbose("AssetLoader", $"Release(string) failed: {assetName} not found in loaded assets");
                return;
            }

            loadedAsset.TryRemove(assetName, out _);
            ResourceLogger.Verbose("AssetLoader", $"Release(string): {assetName} (force={force})");
            AssetsReferenceTree.Instance.Release(lazyAsset.Value, force);
        }

        public GameObject Instantiate(string name)
        {
            ResourceLogger.Verbose("AssetLoader", $"Instantiate: {name}");
            var asset = Load(name);
            if (asset == null)
            {
                throw new System.Exception($"Failed to load asset: {name}");
            }
            if (!asset.Is<GameObject>())
            {
                throw new System.Exception($"{name} not GameObject, please use Get to get it");
            }
            var prefab = asset.As<GameObject>();
            var newObj = Object.Instantiate(prefab);
            var newAsset = new InstantiableAsset(newObj);
            instantiatedGameObject.TryAdd(newObj, newAsset);
            AssetsReferenceTree.Instance.Alloc(asset, newAsset);
            AssetsReferenceTree.Instance.Alloc(newAsset, this);
            ResourceLogger.Verbose("AssetLoader", $"  Instantiated: {name} -> {newObj.name}");
            return newObj;
        }

        public async ValueTask<GameObject> InstantiateAsync(string name)
        {
            ResourceLogger.Verbose("AssetLoader", $"InstantiateAsync: {name}");
            var asset = await LoadAsync(name);
            if (asset == null)
            {
                throw new System.Exception($"Failed to load asset: {name}");
            }
            if (!asset.Is<GameObject>())
            {
                throw new System.Exception($"{name} not GameObject, please use Get to get it");
            }
            var prefab = asset.As<GameObject>();
            var newObj = Object.Instantiate(prefab);
            var newAsset = new InstantiableAsset(newObj);
            instantiatedGameObject.TryAdd(newObj, newAsset);
            AssetsReferenceTree.Instance.Alloc(asset, newAsset);
            AssetsReferenceTree.Instance.Alloc(newAsset, this);
            ResourceLogger.Verbose("AssetLoader", $"  Instantiated async: {name} -> {newObj.name}");
            return newObj;
        }

        public void Destroy(GameObject gameObject, bool force = false)
        {
            if (gameObject == null)
            {
                return;
            }

            if (instantiatedGameObject.TryGetValue(gameObject, out var asset))
            {
                ResourceLogger.Verbose("AssetLoader", $"Destroy: {gameObject.name} (force={force})");
                // 从字典中移除
                instantiatedGameObject.TryRemove(gameObject, out _);
                
                gameObject.transform.SetParent(null);
                gameObject.SetActive(false);
                AssetsReferenceTree.Instance.Release(asset, force);
            }
            else
            {
                ResourceLogger.Verbose("AssetLoader", $"Destroy failed: {gameObject?.name ?? "null"} not found in instantiated objects");
            }
        }

        public void Release()
        {
            ResourceLogger.Verbose("AssetLoader", "Release: AssetLoader instance");
            AssetsReferenceTree.Instance.Release(this);
        }

        void IReference.Release()
        {
            ResourceLogger.Verbose("AssetLoader", $"Release: clearing {loadedAsset.Count} assets, {instantiatedGameObject.Count} game objects");
            loadedAsset.Clear();
            instantiatedGameObject.Clear();
        }
    }
}
