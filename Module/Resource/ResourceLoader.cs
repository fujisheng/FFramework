using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Framework.Module.Resource
{
    class ResourceLoaderPool : ObjectPool<ResourceLoader>
    {
        protected override ResourceLoader New()
        {
            return new ResourceLoader();
        }

        static ResourceLoaderPool instance;

        public static ResourceLoaderPool Instance
        {
            get { return instance ?? (instance = new ResourceLoaderPool()); }
        }
    }
    public class ResourceLoader : IResourceLoader, IDisposable
    {

        bool released = false;
        IResourceManager resourceManager;
        Dictionary<string, Object> cache = new Dictionary<string, Object> ();

        public static ResourceLoader Ctor()
        {
            var loader = ResourceLoaderPool.Instance.Pop();
            loader.released = false;
            return loader;
        }
        

        internal ResourceLoader()
        {
            resourceManager = ModuleManager.Instance.GetModule<IResourceManager>();
        }

        
        void CheckIsReleased()
        {
            if (released)
            {
                throw new Exception("atemp index a released resourceLoader");
            }
        }

        public async Task<T> GetAsync<T>(string assetName) where T : Object
        {
            CheckIsReleased();
            return await resourceManager.LoadAsync<T>(assetName);
        }

        public async Task<IList<T>> GetAllAsync<T>(string label) where T : Object
        {
            CheckIsReleased();
            return await resourceManager.LoadAllAsync<T>(label);
        }

        public async Task<IList<T>> GetAllAsync<T>(IList<string> labelOrNames) where T : Object
        {
            CheckIsReleased();
            return await resourceManager.LoadAllAsync<T>(labelOrNames);
        }

        public async Task Perload<T>(string assetName) where T : Object
        {
            CheckIsReleased();
            if(cache.ContainsKey(assetName))
            {
                return;
            }

            var asset = await GetAsync<T>(assetName);
            cache.Add(assetName, asset);
        }

        public async Task PerloadAll<T>(string label) where T : Object
        {
            CheckIsReleased();
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

        public async Task PerloadAll<T>(IList<string> labelOrNames) where T : Object
        {
            CheckIsReleased();
            var assets = await GetAllAsync<T>(labelOrNames);
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

        public T Get<T>(string assetName) where T : Object
        {
            CheckIsReleased();
            if(cache.TryGetValue(assetName, out Object asset))
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

        public async Task<GameObject> InstantiateAsync(string assetName)
        {
            CheckIsReleased();
            return await resourceManager.InstantiateAsync(assetName);
        }

        public void ReleaseInstance(GameObject gameObject)
        {
            CheckIsReleased();
            resourceManager.ReleaseInstance(gameObject);
        }

        /// <summary>
        /// 释放这个resourceLoader 请在释放后将其滞空
        /// </summary>
        public void Release()
        {
            //TODO 异步加载的东西 是否要释放？？？
            CheckIsReleased();
            cache.Clear();
            released = true;
            ResourceLoaderPool.Instance.Push(this);
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