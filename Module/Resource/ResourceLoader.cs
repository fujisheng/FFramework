using System;
using System.Threading.Tasks;
using UnityEngine;
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

        public static ResourceLoader Ctor()
        {
            var loader = ResourceLoaderPool.Instance.Pop();
            loader.released = false;
            return loader;
        }


        IResourceManager resourceManager;

        public ResourceLoader()
        {
            resourceManager = ModuleManager.Instance.GetModule<IResourceManager>();
        }

        bool released = false;
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

        public T Get<T>(string assetName) where T : Object
        {
            CheckIsReleased();
            var task = resourceManager.LoadAsync<T>(assetName);
            return task.Result;
            //if(task == null)
            //{
            //    return null;
            //}

            //while (true)
            //{
            //    if (task.Result != null)
            //    {
            //        break;
            //    }
            //}
            //return task.Result;
        }

        public GameObject Instantiate(string assetName)
        {
            CheckIsReleased();
            var task = resourceManager.InstantiateAsync(assetName);
            while (true)
            {
                if (task.IsCompleted)
                {
                    break;
                }
            }
            return task.Result;
        }

        public async Task<GameObject> InstantiateAsync(string assetName)
        {
            CheckIsReleased();
            return await resourceManager.InstantiateAsync(assetName);
        }

        public void DestroyGameObject(GameObject gameObject)
        {
            CheckIsReleased();
            Object.DestroyImmediate(gameObject);
        }

        /// <summary>
        /// 释放这个resourceLoader 请在释放后将其滞空
        /// </summary>
        public void Release()
        {
            CheckIsReleased();
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