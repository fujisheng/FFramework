using System.Collections.Concurrent;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Framework.Service.Resource
{
    internal class BundleLoader : Singleton<BundleLoader>, IReference
    {
        AssetBundleManifest manifest;
        ConcurrentDictionary<string, Bundle> bundleCache;

        public BundleLoader()
        {
            bundleCache = new ConcurrentDictionary<string, Bundle>();
            AssetsReferenceTree.Instance.Alloc(AssetsReferenceTree.Root, this);
        }

        protected override void OnConstructor()
        {
            instance.LoadManifest();
        }

        void LoadManifest()
        {
            var bundlePath = $"Assets/AssetsBundle/AssetsBundle";
            var bundle = AssetBundle.LoadFromFile(bundlePath);
            var assetName = bundle.GetAllAssetNames()[0];
            UnityEngine.Debug.Log("LoadManifest");
            manifest = bundle.LoadAsset<AssetBundleManifest>(assetName);
        }

        internal Bundle Load(string bundleName)
        {
            UnityEngine.Debug.Log($"loadBundle:{bundleName}");
            return InternalLoad(bundleName);
        }

        Bundle InternalLoad(string bundleName)
        {
            if (bundleCache.TryGetValue(bundleName, out var bundle))
            {
                return bundle;
            }
            var dependencies = manifest.GetAllDependencies(bundleName);
            foreach (var depend in dependencies)
            {
                UnityEngine.Debug.Log($"depend:{depend}");
                InternalLoad(depend);
            }

            var bundlePath = $"Assets/AssetsBundle/{bundleName}";
            var assetBundle = AssetBundle.LoadFromFile(bundlePath);
            bundle = new Bundle(assetBundle);
            bundleCache.TryAdd(bundleName, bundle);

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

        internal async UniTask<Bundle> LoadAsync(string bundleName, uint crc)
        {
            return await InternalLoadAsync(bundleName);
        }

        async UniTask<Bundle> InternalLoadAsync(string bundleName)
        {
            if (bundleCache.TryGetValue(bundleName, out var bundle))
            {
                return bundle;
            }

            var dependencies = manifest.GetAllDependencies(bundleName);
            foreach (var depend in dependencies)
            {
                UnityEngine.Debug.Log($"depend:{depend}");
                await InternalLoadAsync(depend);
            }

            var bundlePath = $"Assets/AssetsBundle/{bundleName}";
            var assetBundle = await AssetBundle.LoadFromFileAsync(bundlePath);
            bundle = new Bundle(assetBundle);
            bundleCache.TryAdd(bundleName, bundle);

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
                return;
            }
            bundleCache.TryRemove(bundleName, out _);
        }

        void IReference.Release()
        {
            UnityEngine.Debug.Log($"bundleLoaderRelsease");
            bundleCache.Clear();
        }
    }
}