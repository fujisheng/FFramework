using System.Collections.Concurrent;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Service.Resource
{
    internal class Bundle : IReference
    {
        string bundleName;
        AssetBundle bundle;
        readonly ConcurrentDictionary<string, Asset> assetCache;

        internal Bundle(AssetBundle assetBundle)
        {
            assetCache = new ConcurrentDictionary<string, Asset>();
            bundle = assetBundle;
            bundleName = assetBundle.name;
        }

        void IReference.Release()
        {
            assetCache.Clear();
            bundle.Unload(true);
            BundleLoader.Instance.Release(bundleName);
            UnityEngine.Debug.Log($"Bundle:{bundleName} Release");

            bundle = null;
            bundleName = string.Empty;
        }

        internal Asset LoadAsset(string name)
        {
            if(assetCache.TryGetValue(name, out var asset))
            {
                return asset;
            }

            var obj = bundle.LoadAsset<Object>(name);
            asset = new Asset(obj);
            assetCache.TryAdd(name, asset);
            AssetsReferenceTree.Instance.Alloc(this, asset);
            return asset;
        }

        internal async UniTask<Asset> LoadAssetAsync(string name)
        {
            if(assetCache.TryGetValue(name, out var asset))
            {
                return asset;
            }

            var obj = await bundle.LoadAssetAsync<Object>(name);
            asset = new Asset(obj);
            assetCache.TryAdd(name, asset);
            AssetsReferenceTree.Instance.Alloc(this, asset);
            return asset;
        }

        public override string ToString()
        {
            return $"bundle:{bundle?.name}";
        }
    }
}
