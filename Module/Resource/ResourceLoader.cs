using System.Threading.Tasks;
using UnityEngine;

namespace Framework.Module.Resource
{
    public class ResourceLoader : IResourceLoader
    {
        class ResourceLoaderPool : ObjectPool<ResourceLoader>
        {
            protected override ResourceLoader New()
            {
                return new ResourceLoader();
            }
        }

        static ResourceLoaderPool pool;

        public static ResourceLoader Ctor()
        {
            pool = pool ?? (pool = new ResourceLoaderPool());
            return pool.Pop();
        }


        IResourceManager resourceManager;

        ResourceLoader()
        {
            resourceManager = ModuleManager.Instance.GetModule<IResourceManager>();
        }


        public async Task<T> GetAsync<T>(string assetName) where T : Object
        {
            return await resourceManager.LoadAsync<T>(assetName);
        }

        public T Get<T>(string assetName) where T : Object
        {
            return null;
        }

        public GameObject Instantiate(string assetName)
        {
            return null;
        }

        public async Task<GameObject> InstantiateAsync(string assetName)
        {
            return await resourceManager.InstantiateAsync(assetName);
        }

        public void DestroyGameObject(GameObject gameObject)
        {
            Object.DestroyImmediate(gameObject);
        }

        public void Release()
        {
            pool.Push(this);
        }
    }
}