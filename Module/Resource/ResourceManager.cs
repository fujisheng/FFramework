using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Framework.Module.Resource
{
    internal sealed class ResourceManager : Module, IResourceManager
    {
        public async Task<T> LoadAsync<T>(string assetName) where T : Object
        {
            return await Addressables.LoadAssetAsync<T>(assetName).Task;
        }

        public async Task<IList<T>> LoadAllAsync<T>(string label) where T : Object
        {
            return await Addressables.LoadAssetsAsync<T>(label, null).Task;
        }


        public async Task<IList<T>> LoadAllAsync<T>(IList<string> names) where T : Object
        {
            List<Task<T>> tasks = new List<Task<T>>(names.Count);
            IList<T> result = new List<T>(names.Count);
            foreach (var name in names)
            {
                var task = LoadAsync<T>(name);
                tasks.Add(task);
            }
            await Task.WhenAll(tasks);
            foreach (var task in tasks)
            {
                result.Add(task.Result);
            }
            tasks.Clear();
            return result;
        }

        List<object> keys = new List<object>();
        public async Task<IList<T>> LoadAllAsyncWithLabelAndNames<T>(IList<string> labelOrNames) where T : Object
        {
            keys.Clear();
            foreach (var key in labelOrNames)
            {
                keys.Add(key);
            }
            return await Addressables.LoadAssetsAsync<T>(keys, null, Addressables.MergeMode.Intersection).Task;
        }

        public async Task<GameObject> InstantiateAsync(string assetName, Vector3 position = default, Quaternion rotation = default, Transform parent = null, bool trackHandle = true)
        {
            return await Addressables.InstantiateAsync(assetName, position, rotation, parent, trackHandle).Task;
        }

        public void ReleaseInstance(GameObject gameObject)
        {
            Addressables.ReleaseInstance(gameObject);
        }
    }
}