using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Service.Resource
{
    internal class Bundle : IReference
    {
        string bundleName;
        AssetBundle bundle;
        readonly Dictionary<string, Asset> assetCache;

        internal Bundle(AssetBundle assetBundle)
        {
            assetCache = new Dictionary<string, Asset>();
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
            var get = assetCache.TryGetValue(name, out Asset asset);
            if (get)
            {
                return asset;
            }

            var obj = bundle.LoadAsset<Object>(name);
            asset = new Asset(obj);
            assetCache.Add(name, asset);
            AssetsReferenceTree.Instance.Alloc(this, asset);
            return asset;
        }

        public override string ToString()
        {
            return $"bundle:{bundle?.name}";
        }
    }
}
