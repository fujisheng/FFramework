using UnityEngine;

namespace Framework.Module.Resource
{
    public class BundleAsync : Bundle
    {
        private AssetBundleCreateRequest _request;

        public override bool IsDone
        {
            get
            {
                if (LoadState == LoadState.Init)
                    return false;

                if (LoadState == LoadState.Loaded)
                    return true;

                if (LoadState == LoadState.LoadAssetBundle && _request.isDone)
                {
                    asset = _request.assetBundle;
                    LoadState = LoadState.Loaded;
                }

                return _request == null || _request.isDone;
            }
        }

        public override float Progress
        {
            get { return _request != null ? _request.progress : 0f; }
        }

        internal override void Load()
        {
            _request = AssetBundle.LoadFromFileAsync(Name);
            if (_request == null)
            {
                Error = Name + " LoadFromFile failed.";
                return;
            }
            LoadState = LoadState.LoadAssetBundle;
        }

        internal override void Unload()
        {
            if (_request != null)
            {
                _request = null;
            }
            LoadState = LoadState.Unload;
            base.Unload();
        }
    }
}