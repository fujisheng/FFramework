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

        public async Task<IList<T>> LoadAllAsync<T>(IList<string> labelOrNames) where T : Object
        {
            //TODO 优化这个过程
            List<object> keys = new List<object> ();
            foreach(var key in labelOrNames)
            {
                keys.Add(key);
            }
            return await Addressables.LoadAssetsAsync<T>(keys, null, Addressables.MergeMode.Intersection).Task;
        }

        public async Task<GameObject> InstantiateAsync(string assetName)
        {
            return await Addressables.InstantiateAsync(assetName).Task;
        }

        public void ReleaseInstance(GameObject gameObject)
        {
            Addressables.ReleaseInstance(gameObject);
        }
    }
}