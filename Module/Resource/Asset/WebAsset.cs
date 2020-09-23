using UnityEngine;
using UnityEngine.Networking;

namespace Framework.Module.Resource
{
    public class WebAsset : Asset
    {
#if UNITY_2018_3_OR_NEWER
        private UnityWebRequest _www;
#else
        private WWW _www;
#endif

        public override bool IsDone
        {
            get
            {
                if (LoadState == LoadState.Init)
                    return false;
                if (LoadState == LoadState.Loaded)
                    return true;

                if (LoadState == LoadState.LoadAsset)
                {
                    if (_www == null || !string.IsNullOrEmpty(_www.error))
                        return true;

                    if (_www.isDone)
                    {
#if UNITY_2018_3_OR_NEWER
                        if (AssetType != typeof(Texture2D))
                        {
                            if (AssetType != typeof(TextAsset))
                            {
                                if (AssetType != typeof(AudioClip))
                                    Bytes = _www.downloadHandler.data;
                                else
                                    asset = DownloadHandlerAudioClip.GetContent(_www);
                            }
                            else
                            {
                                Text = _www.downloadHandler.text;
                            }
                        }
                        else
                        {
                            asset = DownloadHandlerTexture.GetContent(_www);
                        }
#else
                        if (AssetType != typeof(Texture2D))
                        {
                            if (AssetType != typeof(TextAsset))
                            {
                                if (AssetType != typeof(AudioClip))
                                    Bytes = _www.bytes;
                                else
                                    asset = _www.GetAudioClip();
                            }
                            else
                            {
                                Text = _www.text;
                            }
                        }
                        else
                        {
                            asset = _www.texture;
                        } 
#endif
                        LoadState = LoadState.Loaded;
                        return true;
                    }
                    return false;
                }

                return true;
            }
        }

        public override string Error
        {
            get { return _www.error; }
        }

        public override float Progress
        {
#if UNITY_2018_3_OR_NEWER
            get { return _www.downloadProgress; }
#else
            get { return _www.progress;}
#endif
        }

        internal override void Load()
        {
#if UNITY_2018_3_OR_NEWER
            if (AssetType == typeof(AudioClip))
            {
                _www = UnityWebRequestMultimedia.GetAudioClip(Name, AudioType.WAV);
            }
            else if (AssetType == typeof(Texture2D))
            {
                _www = UnityWebRequestTexture.GetTexture(Name);
            }
            else
            {
                _www = new UnityWebRequest(Name);
                _www.downloadHandler = new DownloadHandlerBuffer();
            }
            _www.SendWebRequest();
#else
            _www = new WWW(name);
#endif
            LoadState = LoadState.LoadAsset;
        }

        internal override void Unload()
        {
            if (asset != null)
            {
                Object.Destroy(asset);
                asset = null;
            }
            if (_www != null)
                _www.Dispose();

            Bytes = null;
            Text = null;
        }
    }
}