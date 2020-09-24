using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework.Module.Resource
{
    public class ResourceLoader : IResourceLoader
    {
        Dictionary<string, IAsset> assets;
        Dictionary<IAsset, int> assetsRef;
        IResourceManager resourceManager;

        public ResourceLoader()
        {
            assets = new Dictionary<string, IAsset>();
            assetsRef = new Dictionary<IAsset, int>();
            resourceManager = ModuleManager.Instance.GetModule<IResourceManager>();
        }

        public void SetResourceManager(IResourceManager resourceManager)
        {
            this.resourceManager = resourceManager;
        }

        public async Task<T> GetAsync<T>(string assetName) where T : Object
        {
            IAsset result;
            bool get = assets.TryGetValue(assetName, out IAsset asset);
            if (get)
            {
                result = asset;
            }
            else
            {
                result = await resourceManager.LoadAsync<T>(assetName);
                if (result == null)
                {
                    return null;
                }
                assets.Add(assetName, result);
            }

            bool getRef = assetsRef.TryGetValue(result, out int refCount);
            if (getRef)
            {
                assetsRef[result] += 1;
            }
            else
            {
                assetsRef.Add(result, 1);
            }

            result.Retain();
            return result.asset as T;
        }

        public T Get<T>(string assetName) where T : Object
        {
            IAsset result;
            bool get = assets.TryGetValue(assetName, out IAsset asset);
            if (get)
            {
                result = asset;
            }
            else
            {
                result = resourceManager.LoadSync<T>(assetName);
                if (result == null)
                {
                    return null;
                }
                assets.Add(assetName, result);
            }

            bool getRef = assetsRef.TryGetValue(result, out int refCount);
            if (getRef)
            {
                assetsRef[result] += 1;
            }
            else
            {
                assetsRef.Add(result, 1);
            }

            result.Retain();
            return result.asset as T;
        }

        public GameObject Instantiate(string assetName)
        {
            IAsset result;
            bool get = assets.TryGetValue(assetName, out IAsset asset);
            if (get)
            {
                result = asset;
            }
            else
            {
                result = resourceManager.LoadSync<GameObject>(assetName);
                if (result == null)
                {
                    return null;
                }
                assets.Add(assetName, result);
            }

            bool getRef = assetsRef.TryGetValue(result, out int refCount);
            if (getRef)
            {
                assetsRef[result] += 1;
            }
            else
            {
                assetsRef.Add(result, 1);
            }

            var obj = Object.Instantiate(result.asset as GameObject);
            result.Require(obj);
            return obj;
        }

        public async Task<GameObject> InstantiateAsync(string assetName)
        {
            return null;
        }

        public void DestroyGameObject(GameObject gameObject)
        {
            Object.DestroyImmediate(gameObject);
        }

        public void Release()
        {
            foreach (var asset in assets.Values)
            {
                int refCount = assetsRef[asset];
                for (int i = 0; i < refCount; i++)
                {
                    asset.Release();
                }
            }
        }
    }
}