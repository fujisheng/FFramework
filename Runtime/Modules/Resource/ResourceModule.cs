using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Module.Resource
{
    /// <summary>
    /// ResourceModule - 已弃用
    /// 请使用 ABResourceModule 替代
    /// </summary>
    [Obsolete("ResourceModule is deprecated. Use ABResourceModule instead.", error: false)]
    internal sealed class ResourceModule : Module, IResourceModule
    {
        ABResourceModule actualModule;

        ABResourceModule GetActualModule()
        {
            if (actualModule == null)
            {
                actualModule = new ABResourceModule();
            }
            return actualModule;
        }

        public GameObject Instantiate(string assetName, Vector3 position = default, Quaternion rotation = default, Transform parent = null, bool trackHandle = true)
        {
            var obj = GetActualModule().Instantiate(assetName);
            if (obj != null)
            {
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                if (parent != null)
                {
                    obj.transform.SetParent(parent);
                }
            }
            return obj;
        }

        public async ValueTask<GameObject> InstantiateAsync(string assetName, Vector3 position = default, Quaternion rotation = default, Transform parent = null, bool trackHandle = true)
        {
            var obj = await GetActualModule().InstantiateAsync(assetName);
            if (obj != null)
            {
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                if (parent != null)
                {
                    obj.transform.SetParent(parent);
                }
            }
            return obj;
        }

        public T Load<T>(string assetName) where T : Object
        {
            return GetActualModule().Load<T>(assetName);
        }

        public IList<T> LoadAll<T>(string group) where T : Object
        {
            return GetActualModule().LoadAll<T>(group);
        }

        public async ValueTask<IList<T>> LoadAllAsync<T>(string group, CancellationToken cancellationToken = default) where T : Object
        {
            cancellationToken.ThrowIfCancellationRequested();
            var result = await GetActualModule().LoadAllAsync<T>(group, cancellationToken);
            return result;
        }

        public async ValueTask<T> LoadAsync<T>(string assetName, CancellationToken cancellationToken = default) where T : Object
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await GetActualModule().LoadAsync<T>(assetName, cancellationToken);
        }

        public void ReleaseInstance(GameObject gameObject)
        {
            if (gameObject != null)
            {
                UnityEngine.Object.Destroy(gameObject);
            }
        }
    }
}