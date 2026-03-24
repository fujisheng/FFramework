using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Module.Resource
{
    public interface IResourceModule
    {
        T Load<T>(string assetName) where T : Object;
        IList<T> LoadAll<T>(string group) where T : Object;

        ValueTask<T> LoadAsync<T>(string assetName, CancellationToken cancellationToken = default) where T : Object;
        ValueTask<IList<T>> LoadAllAsync<T>(string group, CancellationToken cancellationToken = default) where T : Object;

        ValueTask<GameObject> InstantiateAsync(string assetName, Vector3 position = default, Quaternion rotation = default, Transform parent = null, bool trackHandle = true);
        GameObject Instantiate(string assetName, Vector3 position = default, Quaternion rotation = default, Transform parent = null, bool trackHandle = true);

        void ReleaseInstance(GameObject gameObject);
    }
}