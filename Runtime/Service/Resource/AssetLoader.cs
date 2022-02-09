using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using Object = UnityEngine.Object;
using Cysharp.Threading.Tasks;

namespace Framework.Service.Resource
{
    public class AssetLoader : IReference
    {
        Dictionary<string, string> Mapping = new Dictionary<string, string>
        {
            {"Assets/Data/Prefabs/HomeView.prefab","prefabs" },
            {"Assets/Data/Prefabs/TipsView.prefab","prefabs" },
        };

        readonly ConcurrentDictionary<string, Asset> loadedAsset;
        readonly ConcurrentDictionary<GameObject, Asset> instantiatedGameObject;

        public AssetLoader()
        {
            loadedAsset = new ConcurrentDictionary<string, Asset>();
            instantiatedGameObject = new ConcurrentDictionary<GameObject, Asset>();
        }

        Asset Load(string name)
        {
            if (loadedAsset.TryGetValue(name, out var asset))
            {
                return asset;
            }

            if (!Mapping.TryGetValue(name, out var bundleName))
            {
                return null;
            }

            var bundle = BundleLoader.Instance.Load(bundleName);
            asset = bundle.LoadAsset(name);
            loadedAsset.TryAdd(name, asset);
            AssetsReferenceTree.Instance.Alloc(asset, this);
            return asset;
        }

        async UniTask<Asset> LoadAsync(string name)
        {
            if (loadedAsset.TryGetValue(name, out var asset))
            {
                return asset;
            }

            if (!Mapping.TryGetValue(name, out var bundleName))
            {
                return null;
            }

            var bundle = await BundleLoader.Instance.LoadAsync(bundleName, 0);
            asset = await bundle.LoadAssetAsync(name);
            loadedAsset.TryAdd(name, asset);
            AssetsReferenceTree.Instance.Alloc(asset, this);
            return asset;
        }

        public T Load<T>(string name) where T : Object
        {
            var asset = Load(name);
            return asset.As<T>();
        }

        public async UniTask<T> LoadAsync<T>(string name) where T : Object
        {
            var asset = await LoadAsync(name);
            return asset.As<T>();
        }

        public void Release(Object obj, bool force = false)
        {
            if (!loadedAsset.TryGetValue(obj.name, out var asset))
            {
                return;
            }

            loadedAsset.TryRemove(obj.name, out _);
            AssetsReferenceTree.Instance.Release(asset, force);
        }

        public void Release(string assetName, bool force = false)
        {
            if(!loadedAsset.TryGetValue(assetName, out var asset))
            {
                return;
            }

            loadedAsset.TryRemove(assetName, out _);
            AssetsReferenceTree.Instance.Release(asset, force);
        }

        public GameObject Instantiate(string name)
        {
            var asset = Load(name);
            if (!asset.Is<GameObject>())
            {
                throw new System.Exception($"{name} not GameObject, please use Get to get it");
            }
            var prefab = asset.As<GameObject>();
            var newObj = Object.Instantiate(prefab);
            var newAsset = new InstantiableAsset(newObj);
            instantiatedGameObject.TryAdd(newObj, newAsset);
            AssetsReferenceTree.Instance.Alloc(asset, newAsset);
            AssetsReferenceTree.Instance.Alloc(newAsset, this);
            return newObj;
        }

        public async UniTask<GameObject> InstantiateAsync(string name)
        {
            var asset = await LoadAsync(name);
            if (!asset.Is<GameObject>())
            {
                throw new System.Exception($"{name} not GameObject, please use Get to get it");
            }
            var prefab = asset.As<GameObject>();
            var newObj = Object.Instantiate(prefab);
            var newAsset = new InstantiableAsset(newObj);
            instantiatedGameObject.TryAdd(newObj, newAsset);
            AssetsReferenceTree.Instance.Alloc(asset, newAsset);
            AssetsReferenceTree.Instance.Alloc(newAsset, this);
            return newObj;
        }

        public void Destroy(GameObject gameObject, bool force = false)
        {
            if (instantiatedGameObject.TryGetValue(gameObject, out var asset))
            {
                gameObject.transform.SetParent(null);
                gameObject.SetActive(false);
                AssetsReferenceTree.Instance.Release(asset, force);
            }
        }

        public void Release()
        {
            AssetsReferenceTree.Instance.Release(this);
        }

        void IReference.Release()
        {
            UnityEngine.Debug.Log("AssetLoader  Release");
            loadedAsset.Clear();
            instantiatedGameObject.Clear();
        }
    }
}
