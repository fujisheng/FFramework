using System.Threading.Tasks;
using UnityEngine;

namespace Framework.Module.Resource
{
    public interface IResourceManager
    {
        T LoadSync<T>(string assetName) where T : Object;
        Task<T> LoadAsync<T>(string assetName) where T : Object;
        Task<GameObject> InstantiateAsync(string assetName);
    }
}