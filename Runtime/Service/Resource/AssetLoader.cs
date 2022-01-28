using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Service.Resource
{
    public class AssetLoader : IReference
    {
        Dictionary<string, string> Mapping = new Dictionary<string, string>
        {
            {"Assets/Data/Prefabs/HomeView.prefab","prefabs" },
            {"Assets/Data/Prefabs/TipsView.prefab","prefabs" },
        };

        readonly Dictionary<string, Asset> loadedAsset;
        readonly Dictionary<GameObject, Asset> loadedObj;

        public AssetLoader()
        {
            loadedAsset = new Dictionary<string, Asset>();
            loadedObj = new Dictionary<GameObject, Asset>();
        }

        Asset GetAsset(string name)
        {
            if (loadedAsset.TryGetValue(name, out var asset))
            {
                return asset;
            }

            if (!Mapping.TryGetValue(name, out var bundleName))
            {
                return null;
            }

            var bundle = BundleLoader.Instance.LoadBundle(bundleName, 0);
            asset = bundle.LoadAsset(name);
            loadedAsset.Add(name, asset);
            AssetsReferenceTree.Instance.Alloc(asset, this);
            return asset;
        }

        public T Get<T>(string name) where T : Object
        {
            var asset = GetAsset(name);
            return asset.Get<T>();
        }

        public void Release(Object obj, bool force = false)
        {
            if (!loadedAsset.TryGetValue(obj.name, out var asset))
            {
                return;
            }

            loadedAsset.Remove(obj.name);
            AssetsReferenceTree.Instance.Release(asset, force);
        }

        public GameObject Instantiate(string name)
        {
            var asset = GetAsset(name);
            if (!asset.Is<GameObject>())
            {
                throw new System.Exception($"{name} not GameObject, please use Get to get it");
            }
            var prefab = asset.Get<GameObject>();
            var newObj = Object.Instantiate(prefab);
            var newAsset = new InstantiableAsset(newObj);
            loadedObj.Add(newObj, newAsset);
            AssetsReferenceTree.Instance.Alloc(asset, newAsset);
            AssetsReferenceTree.Instance.Alloc(newAsset, this);
            return newObj;
        }

        public void Destroy(GameObject gameObject, bool force = false)
        {
            if (loadedObj.TryGetValue(gameObject, out var asset))
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
            UnityEngine.Debug.Log("ResourcesLoader  Release");
            loadedAsset.Clear();
            loadedObj.Clear();
        }
    }
}
