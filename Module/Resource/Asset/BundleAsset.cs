using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Framework.Module.Resource
{
    public class BundleAsset : Asset
    {
        protected readonly string assetBundleName;
        protected Bundle bundle;
        protected IBundleLoader bundleLoader;

        public BundleAsset(string bundle, IBundleLoader bundleLoader)
        {
            assetBundleName = bundle;
            this.bundleLoader = bundleLoader;
        }

        internal override void Load()
        {
            bundle = bundleLoader.Load(assetBundleName);
            var assetName = Path.GetFileName(Name);
            asset = bundle.assetBundle.LoadAsset(assetName, AssetType);
        }

        internal override void Unload()
        {
            if (bundle != null)
            {
                bundle.Release();
                bundle = null;
            }

            asset = null;
        }
    }
}