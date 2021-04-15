using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Framework.Service.Resource
{
    internal sealed class ResourceService : Service, IResourceService
    {
        /// <summary>
        /// 根据资源名字异步加载资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="assetName">资源名字</param>
        /// <returns>资源</returns>
        public async UniTask<T> LoadAsync<T>(string assetName) where T : Object
        {
            return await Addressables.LoadAssetAsync<T>(assetName).Task;
        }

        /// <summary>
        /// 根据某个label异步加载所有资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="label">标签</param>
        /// <returns>所有为这个标签的资源</returns>
        public async UniTask<IList<T>> LoadAllAsync<T>(string label) where T : Object
        {
            return await Addressables.LoadAssetsAsync<T>(label, null).Task;
        }

        /// <summary>
        /// 异步加载所有名字的资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="names">要加载的资源的名字</param>
        /// <returns>这些资源</returns>
        public async UniTask<IList<T>> LoadAllAsync<T>(IList<string> names) where T : Object
        {
            List<UniTask<T>> tasks = new List<UniTask<T>>(names.Count);
            IList<T> result = new List<T>(names.Count);
            foreach (var name in names)
            {
                var task = LoadAsync<T>(name);
                tasks.Add(task);
            }
            await UniTask.WhenAll(tasks);
            foreach (var task in tasks)
            {
                result.Add(task.AsTask().Result);
            }
            tasks.Clear();
            return result;
        }

        List<object> keys = new List<object>();
        /// <summary>
        /// 根据标签和名字异步加载资源
        /// </summary>
        /// <typeparam name="T">资源名字</typeparam>
        /// <param name="labelOrNames">标签和名字</param>
        /// <returns>所有的资源</returns>
        public async UniTask<IList<T>> LoadAllAsyncWithLabelAndNames<T>(IList<string> labelOrNames) where T : Object
        {
            keys.Clear();
            foreach (var key in labelOrNames)
            {
                keys.Add(key);
            }
            return await Addressables.LoadAssetsAsync<T>(keys, null, Addressables.MergeMode.Intersection).Task;
        }

        /// <summary>
        /// 根据名字异步实例化prefab
        /// </summary>
        /// <param name="assetName">资源名字</param>
        /// <param name="position">位置</param>
        /// <param name="rotation">旋转</param>
        /// <param name="parent">父物体</param>
        /// <param name="trackHandle"></param>
        /// <returns></returns>
        public async UniTask<GameObject> InstantiateAsync(string assetName, Vector3 position = default, Quaternion rotation = default, Transform parent = null, bool trackHandle = true)
        {
            return await Addressables.InstantiateAsync(assetName, position, rotation, parent, trackHandle).Task;
        }

        /// <summary>
        /// 销毁一个gameObject
        /// </summary>
        /// <param name="gameObject">要销毁的gameObject</param>
        public void ReleaseInstance(GameObject gameObject)
        {
            Addressables.ReleaseInstance(gameObject);
        }
    }
}