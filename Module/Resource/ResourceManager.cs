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

        public async Task<GameObject> InstantiateAsync(string assetName)
        {
            return await Addressables.InstantiateAsync(assetName).Task;
        }

        public async void InstantiateGameObjectWithCallback(string assetName, Action<GameObject> onInstantiated)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                Debug.LogWarning("尝试实例化一个名字为空的gameObject！！！");
                return;
            }

            onInstantiated?.Invoke(await InstantiateAsync(assetName));
        }

        public void Destroy(GameObject gameObject)
        {
            Addressables.ReleaseInstance(gameObject);
        }
    }
}