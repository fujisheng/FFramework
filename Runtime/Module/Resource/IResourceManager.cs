using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Module.Resource
{
    public interface IResourceManager
    {
        UniTask<T> LoadAsync<T>(string assetName) where T : Object;
        UniTask<IList<T>> LoadAllAsync<T>(string lable) where T : Object;
        UniTask<IList<T>> LoadAllAsync<T>(IList<string> names) where T : Object;
        UniTask<IList<T>> LoadAllAsyncWithLabelAndNames<T>(IList<string> labelAndNames) where T : Object;
        UniTask<GameObject> InstantiateAsync(string assetName, Vector3 position = default, Quaternion rotation = default, Transform parent = null, bool trackHandle = true);
        void ReleaseInstance(GameObject gameObject);
    }
}