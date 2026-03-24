using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework.Module.Resource
{
    internal class BundleLoader : Singleton<BundleLoader>, IReference
    {
        AssetBundleManifest manifest;
        ConcurrentDictionary<string, Bundle> bundleCache;

        private BundleLoader()
        {
            bundleCache = new ConcurrentDictionary<string, Bundle>();
            AssetsReferenceTree.Instance.Alloc(AssetsReferenceTree.Root, this);

            LoadManifest();
        }

        void LoadManifest()
        {
            var bundlePath = $"Assets/AssetsBundle/AssetsBundle";
            var bundle = AssetBundle.LoadFromFile(bundlePath);
            if (bundle == null)
            {
                throw new System.Exception($"Failed to load manifest bundle from {bundlePath}");
            }
            var assetNames = bundle.GetAllAssetNames();
            if (assetNames == null || assetNames.Length == 0)
            {
                throw new System.Exception("Manifest bundle contains no assets");
            }
            var assetName = assetNames[0];
            ResourceLogger.Verbose("BundleLoader", $"LoadManifest: {assetName}");
            manifest = bundle.LoadAsset<AssetBundleManifest>(assetName);
            if (manifest == null)
            {
                throw new System.Exception($"Failed to load AssetBundleManifest from {assetName}");
            }
        }

        internal Bundle Load(string bundleName)
        {
            ResourceLogger.Verbose("BundleLoader", $"Load: {bundleName}");
            return InternalLoad(bundleName);
        }

        Bundle InternalLoad(string bundleName)
        {
            if (bundleCache.TryGetValue(bundleName, out var bundle))
            {
                ResourceLogger.Verbose("BundleLoader", $"Load from cache: {bundleName}");
                return bundle;
            }

            var dependencies = manifest.GetAllDependencies(bundleName);
            ResourceLogger.Verbose("BundleLoader", $"InternalLoad: {bundleName}, dependencies: {dependencies.Length}");
            foreach (var depend in dependencies)
            {
                ResourceLogger.Verbose("BundleLoader", $"  Loading dependency: {depend}");
                InternalLoad(depend);
            }

            var bundlePath = $"Assets/AssetsBundle/{bundleName}";
            var assetBundle = AssetBundle.LoadFromFile(bundlePath);
            if (assetBundle == null)
            {
                throw new System.Exception($"Failed to load AssetBundle from {bundlePath}");
            }
            bundle = new Bundle(assetBundle);
            bundleCache.TryAdd(bundleName, bundle);
            ResourceLogger.Verbose("BundleLoader", $"  Loaded bundle: {bundleName}");

            var alloc = false;
            foreach (var dependName in dependencies)
            {
                if (bundleCache.TryGetValue(dependName, out var dependBundle))
                {
                    AssetsReferenceTree.Instance.Alloc(dependBundle, bundle);
                    alloc = true;
                }
            }

            if (!alloc)
            {
                AssetsReferenceTree.Instance.Alloc(this, bundle);
            }

            return bundle;
        }

        internal async ValueTask<Bundle> LoadAsync(string bundleName, uint crc, CancellationToken cancellationToken = default)
        {
            ResourceLogger.Verbose("BundleLoader", $"LoadAsync: {bundleName}, crc={crc}");
            return await InternalLoadAsync(bundleName, cancellationToken);
        }

        async ValueTask<Bundle> InternalLoadAsync(string bundleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            if (bundleCache.TryGetValue(bundleName, out var bundle))
            {
                ResourceLogger.Verbose("BundleLoader", $"LoadAsync from cache: {bundleName}");
                return bundle;
            }

            var dependencies = manifest.GetAllDependencies(bundleName);
            ResourceLogger.Verbose("BundleLoader", $"InternalLoadAsync: {bundleName}, dependencies: {dependencies.Length}");
            // 并行加载所有依赖
            if (dependencies.Length > 0)
            {
                var dependencyTasks = new ValueTask<Bundle>[dependencies.Length];
                for (int i = 0; i < dependencies.Length; i++)
                {
                    ResourceLogger.Verbose("BundleLoader", $"  Loading dependency: {dependencies[i]}");
                    dependencyTasks[i] = InternalLoadAsync(dependencies[i], cancellationToken);
                }
                
                // 等待所有依赖并行加载完成
                await Task.WhenAll(dependencyTasks.Select(vt => vt.AsTask()).ToArray());
            }

            var bundlePath = $"Assets/AssetsBundle/{bundleName}";
            var assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(bundlePath);
            await assetBundleCreateRequest;
            cancellationToken.ThrowIfCancellationRequested();
            var assetBundle = assetBundleCreateRequest.assetBundle;
            bundle = new Bundle(assetBundle);
            bundleCache.TryAdd(bundleName, bundle);
            ResourceLogger.Verbose("BundleLoader", $"  Loaded bundle async: {bundleName}");

            var alloc = false;
            foreach (var dependName in dependencies)
            {
                if (bundleCache.TryGetValue(dependName, out var dependBundle))
                {
                    AssetsReferenceTree.Instance.Alloc(dependBundle, bundle);
                    alloc = true;
                }
            }

            if (!alloc)
            {
                AssetsReferenceTree.Instance.Alloc(this, bundle);
            }

            return bundle;
        }

        internal void Release(string bundleName)
        {
            if (!bundleCache.TryGetValue(bundleName, out _))
            {
                ResourceLogger.Verbose("BundleLoader", $"Release failed: {bundleName} not found in cache");
                return;
            }
            bundleCache.TryRemove(bundleName, out _);
            ResourceLogger.Verbose("BundleLoader", $"Release: {bundleName}, cache size: {bundleCache.Count}");
        }

        void IReference.Release()
        {
            ResourceLogger.Verbose("BundleLoader", $"Release: clearing {bundleCache.Count} bundles");
            bundleCache.Clear();
        }
    }
}