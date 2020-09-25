using Framework.Awaiting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;
using WaitUntil = Framework.Awaiting.WaitUntil;

namespace Framework.Module.Resource
{
    internal sealed class ResourcesManager : Module, IResourceManager
    {
        Dictionary<string, string> mapping;

        internal override async Task OnLoad()
        {
            mapping = (Resources.Load("GameSetting/AssetsNameMapping") as AssetsNameMapping).mapping;
            await base.OnLoad();
        }
        public async Task<IAsset> LoadAsync<T>(string assetName) where T : Object
        {
            string path = mapping[assetName];
            path = RemoveEx(path);
            string resultPath = path.Replace("Assets/Resources/", "");
            ResourceRequest request = Resources.LoadAsync<T>(resultPath);
            await new WaitUntil(() => request.isDone);
            Object obj = request.asset;
            Asset asset = new Asset
            {
                asset = obj,
                AssetType = typeof(T)
            };
            return asset;
        }

        public IAsset LoadSync<T>(string assetName) where T : Object
        {
            string path = mapping[assetName];
            path = RemoveEx(path);
            string resultPath = path.Replace("Assets/Resources/", "");
            Object obj = Resources.Load<T>(resultPath);
            Asset asset = new Asset
            {
                asset = obj,
                AssetType = typeof(T)
            };
            return asset;
        }

        public async void InstantiateGameObjectWithCallback(string assetName, Action<GameObject> onInstantiated)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                Debug.LogWarning("尝试实例化一个名字为空的gameObject！！！");
                return;
            }
            string path = mapping[assetName];
            path = RemoveEx(path);
            string resultPath = path.Replace("Assets/Resources/", "");
            ResourceRequest request = Resources.LoadAsync(resultPath);
            await new WaitUntil(() => request.isDone);
            Object obj = request.asset;
            var result = Object.Instantiate(request.asset);
            onInstantiated?.Invoke(result as GameObject);
        }

        string RemoveEx(string path)
        {
            path = path.Replace(".txt", "");
            path = path.Replace(".asset", "");
            path = path.Replace(".mp3", "");
            path = path.Replace(".mp4", "");
            path = path.Replace(".png", "");
            path = path.Replace(".jpg", "");
            path = path.Replace(".prefab", "");
            return path;
        }
    }
}