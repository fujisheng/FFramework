using System.Collections.Concurrent;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Module.Resource
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
            if (obj == null)
            {
                UnityEngine.Debug.LogError($"Failed to load asset '{name}' from bundle '{bundleName}'");
                return null;
            }
            asset = new Asset(obj);
            assetCache.TryAdd(name, asset);
            AssetsReferenceTree.Instance.Alloc(this, asset);
            return asset;
        }

        internal async ValueTask<Asset> LoadAssetAsync(string name)
        {
            if (assetCache.TryGetValue(name, out var asset))
            {
                return asset;
            }

            var request = bundle.LoadAssetAsync<Object>(name);
            await request;
            var obj = request.asset;
            if (obj == null)
            {
                UnityEngine.Debug.LogError($"Failed to load asset '{name}' from bundle '{bundleName}'");
                return null;
            }
            asset = new Asset(obj);
            assetCache.TryAdd(name, asset);
            AssetsReferenceTree.Instance.Alloc(this, asset);
            return asset;
        }

        /// <summary>
        /// 获取此 Bundle 中所有资源的名称
        /// </summary>
        internal string[] GetAllAssetNames()
        {
            if (bundle == null)
            {
                return new string[0];
            }
            return bundle.GetAllAssetNames();
        }

        public override string ToString()
        {
            return $"bundle:{bundle?.name}";
        }
    }
}
