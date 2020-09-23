using UnityEngine;
using UnityEngine.Networking;

namespace Framework.Module.Resource
{
    public class WebBundle : Bundle
    {
#if UNITY_2018_3_OR_NEWER
        private UnityWebRequest _request;


#else
		private WWW _request;
#endif
        public bool cache;
        public Hash128 hash;

        public override string Error
        {
            get { return _request != null ? _request.error : null; }
        }

        public override bool IsDone
        {
            get
            {
                if (LoadState == LoadState.Init)
                    return false;

                if (_request == null || LoadState == LoadState.Loaded)
                    return true;
#if UNITY_2018_3_OR_NEWER
                if (_request.isDone)
                {
                    assetBundle = DownloadHandlerAssetBundle.GetContent(_request);
                    LoadState = LoadState.Loaded;
                }
#else
                if (_request.isDone)
                {
                    assetBundle = _request.assetBundle;
                    LoadState = LoadState.Loaded;
                }
#endif

                return _request.isDone;
            }
        }

        public override float Progress
        {
#if UNITY_2018_3_OR_NEWER
            get { return _request != null ? _request.downloadProgress : 0f; }
#else
			get { return _request != null ? _request.progress : 0f; }
#endif
        }

        internal override void Load()
        {
#if UNITY_2018_3_OR_NEWER
            _request = cache ? UnityWebRequestAssetBundle.GetAssetBundle(Name, hash) : UnityWebRequestAssetBundle.GetAssetBundle(Name);
            _request.SendWebRequest();
#else
            _request = cache ? WWW.LoadFromCacheOrDownload(name, hash) : new WWW(name);
#endif
            LoadState = LoadState.LoadAssetBundle;

        }

        internal override void Unload()
        {
            if (_request != null)
            {
                _request.Dispose();
                _request = null;
            }
            LoadState = LoadState.Unload;
            base.Unload();
        }
    }
}