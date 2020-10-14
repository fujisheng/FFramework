using System.Threading.Tasks;
using UnityEngine;

namespace Framework.Module.Resource
{
    public interface IResourceManager
    {
        Task<T> LoadAsync<T>(string assetName) where T : Object;
        Task<GameObject> InstantiateAsync(string assetName);
    }
}