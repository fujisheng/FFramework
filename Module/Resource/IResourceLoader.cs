using System.Threading.Tasks;
using UnityEngine;

namespace Framework.Module.Resource
{
    public interface IResourceLoader
    {
        void SetResourceManager(IResourceManager resourceManager);
        Task<T> GetAsync<T>(string assetName) where T : Object;
        T Get<T>(string assetName) where T : Object;
        GameObject Instantiate(string assetName);
        void Release();
    }
}