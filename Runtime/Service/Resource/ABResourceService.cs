using UnityEngine;
using System.Collections.Concurrent;
using Cysharp.Threading.Tasks;

namespace Framework.Service.Resource
{
    internal class ABResourceService : Service
    {
        float lastCollectTime = 0f;
        ConcurrentDictionary<string, AssetLoader> loaderMapping = new ConcurrentDictionary<string, AssetLoader>();

        public T Load<T>(string name) where T : Object
        {
            var group = GetGroup(name);
            var loader = GetOrCreateLoader(group);
            return loader.Load<T>(name);
        }

        public T Load<T>(string groupName, string name) where T : Object
        {
            var loader = GetOrCreateLoader(groupName);
            return loader.Load<T>(name);
        }

        public async UniTask<T> LoadAsync<T>(string name) where T : Object
        {
            var group = GetGroup(name);
            var loader = GetOrCreateLoader(group);
            return await loader.LoadAsync<T>(name);
        }

        public async UniTask<T> LoadAsync<T>(string groupName, string assetName) where T : Object
        {
            var loader = GetOrCreateLoader(groupName);
            return await loader.LoadAsync<T>(assetName);
        }

        public GameObject Instantiate(string name)
        {
            var group = GetGroup(name);
            var loader = GetOrCreateLoader(group);
            return loader.Instantiate(name);
        }

        public async UniTask<GameObject> InstantiateAsync(string name)
        {
            var group = GetGroup(name);
            var loader = GetOrCreateLoader(group);
            return await loader.InstantiateAsync(name);
        }

        string GetGroup(string assetName)
        {
            return string.Empty;
        }

        AssetLoader GetOrCreateLoader(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                UnityEngine.Debug.LogWarning($"ResourceGroup is empty, default assetLoader will be used!!!");
            }

            if(loaderMapping.TryGetValue(groupName, out var loader))
            {
                return loader;
            }
            loader = new AssetLoader();
            loaderMapping.TryAdd(groupName, loader);
            return loader;
        }

        public void ReleaseAsset(string assetName)
        {
            var group = GetGroup(assetName);
            if(!loaderMapping.TryGetValue(group, out var loader))
            {
                return;
            }
            loader.Release(assetName);
        }

        public void ReleaseGroup(string groupName)
        {
            if(!loaderMapping.TryGetValue(groupName, out var loader))
            {
                return;
            }
            loader.Release();
        }

        internal override void OnLateUpdate()
        {
            if(Time.realtimeSinceStartup - lastCollectTime > 20f)
            {
                AssetsReferenceTree.Instance.Collect();
                AssetsReferenceTree.Instance.Delete();
                lastCollectTime = Time.realtimeSinceStartup;
            }
        }
    }
}
