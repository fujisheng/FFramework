using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Module.Resource
{
    public class ResourceLoader : IResourceLoader, IDisposable
    {
        IResourceManager resourceManager;
        Dictionary<string, Object> cache = new Dictionary<string, Object> ();
        
        public ResourceLoader()
        {
            resourceManager = ModuleManager.Instance.GetModule<IResourceManager>();
            if(resourceManager == null)
            {
                throw new Exception("Please add module [ResourceManager] before using ResourceLoader");
            }
        }

        public async UniTask<T> GetAsync<T>(string assetName) where T : Object
        {
            return await resourceManager.LoadAsync<T>(assetName);
        }

        public async UniTask<IList<T>> GetAllAsync<T>(string label) where T : Object
        {
            return await resourceManager.LoadAllAsync<T>(label);
        }

        public async UniTask<IList<T>> GetAllAsync<T>(IList<string> names) where T : Object
        {
            return await resourceManager.LoadAllAsync<T>(names);
        }

        public async UniTask<IList<T>> GetAllAsyncWithLabelAndNames<T>(IList<string> labelAndNames) where T : Object
        {
            return await resourceManager.LoadAllAsyncWithLabelAndNames<T>(labelAndNames);
        }

        public async UniTask Perload<T>(string assetName) where T : Object
        {
            if(cache.ContainsKey(assetName))
            {
                return;
            }

            var asset = await GetAsync<T>(assetName);
            cache.Add(assetName, asset);
        }

        public async UniTask PerloadAll<T>(string label) where T : Object
        {
            var assets = await GetAllAsync<T>(label);
            foreach(var asset in assets)
            {
                var assetName = asset.name;
                if(cache.ContainsKey(assetName))
                {
                    continue;
                }
                cache.Add(assetName, asset);
            }
        }

        public async UniTask PerloadAll<T>(IList<string> names) where T : Object
        {
            var assets = await GetAllAsync<T>(names);
            foreach(var asset in assets)
            {
                var assetName = asset.name;
                if(cache.ContainsKey(assetName))
                {
                    continue;
                }
                cache.Add(assetName, asset);
            }
        }

        public async UniTask PerloadAllWithLabelAndNames<T>(IList<string> labelAndNames) where T : Object
        {
            var assets = await GetAllAsyncWithLabelAndNames<T>(labelAndNames);
            foreach (var asset in assets)
            {
                var assetName = asset.name;
                if (cache.ContainsKey(assetName))
                {
                    continue;
                }
                cache.Add(assetName, asset);
            }
        }

        public T Get<T>(string assetName) where T : Object
        {
            if (cache.TryGetValue(assetName, out Object asset))
            {
                if(asset is GameObject)
                {
                    throw new Exception("It is not allowed to get GameObject through this method. If you want to create GameObject, please use InstantiateAsync");
                }
                return asset as T;
            }
            Debug.LogWarning($"Try to get a resource without preloading synchronously : {assetName}");
            return null;
        }

        public async UniTask<GameObject> InstantiateAsync(string assetName, Vector3 position = default, Quaternion rotation = default, Transform parent = null, bool trackHandle = true)
        {
            return await resourceManager.InstantiateAsync(assetName, position, rotation, parent, trackHandle);
        }

        public void ReleaseInstance(GameObject gameObject)
        {
            resourceManager.ReleaseInstance(gameObject);
        }

        /// <summary>
        /// 释放这个resourceLoader 请在释放后将其滞空
        /// </summary>
        public void Release()
        {
            //TODO 异步加载的东西 是否要释放？？？
            cache.Clear();
        }

        public void Dispose()
        {
            Release();
        }

        ~ResourceLoader()
        {
            Release();
        }
    }
}