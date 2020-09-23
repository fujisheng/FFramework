using System;
using System.IO;
using UnityEngine;

namespace Framework.Module.Resource
{
    public class BundleAssetAsync : BundleAsset
    {
        private AssetBundleRequest _request;
        public BundleAssetAsync(string bundle, IBundleLoader bundleLoader) : base(bundle, bundleLoader) { }

        public override bool IsDone
        {
            get
            {
                if (Error != null || bundle.Error != null)
                {
                    return true;
                } 

                for (int i = 0, max = bundle.dependencies.Count; i < max; i++)
                {
                    var item = bundle.dependencies[i];
                    if (item.Error != null)
                    {
                        return true;
                    }   
                }

                switch (LoadState)
                {
                    case LoadState.Init:
                        return false;
                    case LoadState.Loaded:
                        return true;
                    case LoadState.LoadAssetBundle:
                        if (!bundle.IsDone)
                        {
                            return false;
                        }
                            
                        for (int i = 0, max = bundle.dependencies.Count; i < max; i++)
                        {
                            var item = bundle.dependencies[i];
                            if (!item.IsDone)
                            {
                                return false;
                            }   
                        }

                        if (bundle.assetBundle == null)
                        {
                            Error = "assetBundle == null";
                            return true;
                        }

                        var assetName = Path.GetFileName(Name);
                        _request = bundle.assetBundle.LoadAssetAsync(assetName, AssetType);
                        LoadState = LoadState.LoadAsset;
                        break;
                    case LoadState.Unload:
                        break;
                    case LoadState.LoadAsset:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (LoadState != LoadState.LoadAsset)
                {
                    return false;
                }
                    
                if (!_request.isDone)
                {
                    return false;
                }
                    
                asset = _request.asset;
                LoadState = LoadState.Loaded;
                return true;
            }
        }

        public override float Progress
        {
            get
            {
                var bundleProgress = bundle.Progress;
                if (bundle.dependencies.Count <= 0)
                {
                    return bundleProgress * 0.3f + (_request != null ? _request.progress * 0.7f : 0);
                }
                    
                for (int i = 0, max = bundle.dependencies.Count; i < max; i++)
                {
                    var item = bundle.dependencies[i];
                    bundleProgress += item.Progress;
                }

                return bundleProgress / (bundle.dependencies.Count + 1) * 0.3f +
                       (_request != null ? _request.progress * 0.7f : 0);
            }
        }

        internal override void Load()
        {
            bundle = bundleLoader.LoadAsync(assetBundleName);
            LoadState = LoadState.LoadAssetBundle;
        }

        internal override void Unload()
        {
            _request = null;
            LoadState = LoadState.Unload;
            base.Unload();
        }
    }
}