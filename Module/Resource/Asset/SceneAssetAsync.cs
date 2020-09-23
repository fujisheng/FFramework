using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework.Module.Resource
{
    public class SceneAssetAsync : SceneAsset
    {
        private AsyncOperation _request;

        public SceneAssetAsync(string path, bool addictive, IBundleLoader bundleLoader): base(path, addictive, bundleLoader){ }

        public override float Progress
        {
            get
            {
                if (bundle == null)
                    return _request == null ? 0 : _request.progress;

                var bundleProgress = bundle.Progress;
                if (bundle.dependencies.Count <= 0)
                    return bundleProgress * 0.3f + (_request != null ? _request.progress * 0.7f : 0);
                for (int i = 0, max = bundle.dependencies.Count; i < max; i++)
                {
                    var item = bundle.dependencies[i];
                    bundleProgress += item.Progress;
                }

                return bundleProgress / (bundle.dependencies.Count + 1) * 0.3f +
                       (_request != null ? _request.progress * 0.7f : 0);
            }
        }

        public override bool IsDone
        {
            get
            {
                switch (LoadState)
                {
                    case LoadState.Loaded:
                        return true;
                    case LoadState.LoadAssetBundle:
                        {
                            if (bundle == null || bundle.Error != null)
                                return true;

                            for (int i = 0, max = bundle.dependencies.Count; i < max; i++)
                            {
                                var item = bundle.dependencies[i];
                                if (item.Error != null)
                                    return true;
                            }

                            if (!bundle.IsDone)
                                return false;

                            for (int i = 0, max = bundle.dependencies.Count; i < max; i++)
                            {
                                var item = bundle.dependencies[i];
                                if (!item.IsDone)
                                    return false;
                            }

                            _request = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
                            LoadState = LoadState.LoadAsset;
                            break;
                        }
                    case LoadState.Unload:
                        break;
                    case LoadState.LoadAsset:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (LoadState != LoadState.LoadAsset)
                    return false;
                if (!_request.isDone)
                    return false;
                LoadState = LoadState.Loaded;
                return true;
            }
        }

        internal override void Load()
        {
            if (!string.IsNullOrEmpty(assetBundleName))
            {
                bundle = bundleLoader.LoadAsync(assetBundleName);
                LoadState = LoadState.LoadAssetBundle;
            }
            else
            {
                _request = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
                LoadState = LoadState.LoadAsset;
            }
        }

        internal override void Unload()
        {
            base.Unload();
            _request = null;
        }
    }
}

