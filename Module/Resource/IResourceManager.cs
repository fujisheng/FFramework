using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;

namespace Framework.Module.Resource
{
    public interface IResourceManager
    {
        Task<T> LoadAsync<T>(string assetName) where T : Object;
        Task<IList<T>> LoadAllAsync<T>(string lable) where T : Object;
        Task<GameObject> InstantiateAsync(string assetName);
    }
}