using Cysharp.Threading.Tasks;
using FInject;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Module.Resource
{
    /// <summary>
    /// 资源加载器
    /// </summary>
    public class ResourceLoader : IResourceLoader, IDisposable
    {
        [Inject]
        public IResourceManager resourceManager { set; private get; }
        Dictionary<string, Object> cache = new Dictionary<string, Object> ();

        void CheckResourceManager()
        {
            if (resourceManager == null)
            {
                throw new Exception("Please add module [ResourceManager] before using ResourceLoader");
            }
        }

        /// <summary>
        /// 根据资源名字异步获取某个资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="assetName">资源名字</param>
        /// <returns>资源</returns>
        public async UniTask<T> GetAsync<T>(string assetName) where T : Object
        {
            CheckResourceManager();
            return await resourceManager.LoadAsync<T>(assetName);
        }

        /// <summary>
        /// 根据标签异步获取所有资源啊
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="label">资源标签</param>
        /// <returns>所有为这个标签的资源</returns>
        public async UniTask<IList<T>> GetAllAsync<T>(string label) where T : Object
        {
            CheckResourceManager();
            return await resourceManager.LoadAllAsync<T>(label);
        }

        /// <summary>
        /// 根据多个名字异步获取这些资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="names">要获取的资源的名字</param>
        /// <returns>所有的资源</returns>
        public async UniTask<IList<T>> GetAllAsync<T>(IList<string> names) where T : Object
        {
            CheckResourceManager();
            return await resourceManager.LoadAllAsync<T>(names);
        }

        /// <summary>
        /// 根据标签和名字获取所有资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="labelAndNames">标签和名字</param>
        /// <returns>满足这些标签和名字的资源</returns>
        public async UniTask<IList<T>> GetAllAsyncWithLabelAndNames<T>(IList<string> labelAndNames) where T : Object
        {
            CheckResourceManager();
            return await resourceManager.LoadAllAsyncWithLabelAndNames<T>(labelAndNames);
        }

        /// <summary>
        /// 预加载某个名字的资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="assetName">资源名字</param>
        /// <returns>task</returns>
        public async UniTask Perload<T>(string assetName) where T : Object
        {
            CheckResourceManager();
            if (cache.ContainsKey(assetName))
            {
                return;
            }

            var asset = await GetAsync<T>(assetName);
            cache.Add(assetName, asset);
        }

        /// <summary>
        /// 根据标签预加载所有的资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="label">标签</param>
        /// <returns>task</returns>
        public async UniTask PerloadAll<T>(string label) where T : Object
        {
            CheckResourceManager();
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

        /// <summary>
        /// 预加载所有名字的资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="names">要预加载的资源的名字</param>
        /// <returns>task</returns>
        public async UniTask PerloadAll<T>(IList<string> names) where T : Object
        {
            CheckResourceManager();
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

        /// <summary>
        /// 通过标签和名字预加载资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="labelAndNames">标签和名字</param>
        /// <returns>task</returns>
        public async UniTask PerloadAllWithLabelAndNames<T>(IList<string> labelAndNames) where T : Object
        {
            CheckResourceManager();
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

        /// <summary>
        /// 从预加载的资源中同步获取某个资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="assetName">资源名字</param>
        /// <returns>资源</returns>
        public T Get<T>(string assetName) where T : Object
        {
            CheckResourceManager();
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

        /// <summary>
        /// 异步实例化某个名字的prefab
        /// </summary>
        /// <param name="assetName">资源名字</param>
        /// <param name="position">位置</param>
        /// <param name="rotation">旋转</param>
        /// <param name="parent">父物体</param>
        /// <param name="trackHandle"></param>
        /// <returns></returns>
        public async UniTask<GameObject> InstantiateAsync(string assetName, Vector3 position = default, Quaternion rotation = default, Transform parent = null, bool trackHandle = true)
        {
            CheckResourceManager();
            return await resourceManager.InstantiateAsync(assetName, position, rotation, parent, trackHandle);
        }

        /// <summary>
        /// 销毁某个gameObject
        /// </summary>
        /// <param name="gameObject">要销毁的gameObject</param>
        public void ReleaseInstance(GameObject gameObject)
        {
            CheckResourceManager();
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