using System.Collections.Generic;
using UnityEngine;

namespace Framework.Service.Resource
{
    internal class BundleLoader : Singleton<BundleLoader>, IReference
    {
        AssetBundleManifest manifest;
        Dictionary<string, Bundle> bundleCache;

        private BundleLoader()
        {
            bundleCache = new Dictionary<string, Bundle>();
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
            manifest = bundle.LoadAsset<AssetBundleManifest>(assetName);
        }

        internal Bundle LoadBundle(string bundleName, uint crc)
        {
            UnityEngine.Debug.Log($"loadBundle:{bundleName}");
            return InternalLoadBundle(bundleName);
        }

        Bundle InternalLoadBundle(string bundleName)
        {
            if (bundleCache.TryGetValue(bundleName, out var bundle))
            {
                return bundle;
            }
            var dependencies = manifest.GetAllDependencies(bundleName);
            foreach (var depend in dependencies)
            {
                UnityEngine.Debug.Log($"depend:{depend}");
                InternalLoadBundle(depend);
            }

            var bundlePath = $"Assets/AssetsBundle/{bundleName}";
            var assetBundle = AssetBundle.LoadFromFile(bundlePath);
            bundle = new Bundle(assetBundle);
            bundleCache.Add(bundleName, bundle);

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
            bundleCache.Remove(bundleName);
        }

        void IReference.Release()
        {
            UnityEngine.Debug.Log($"bundleLoaderRelsease");
            bundleCache.Clear();
        }
    }
}