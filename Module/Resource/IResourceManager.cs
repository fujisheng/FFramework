using System.Threading.Tasks;
using UnityEngine;

namespace Framework.Module.Resource
{
    public interface IResourceManager
    {
        IAsset LoadSync<T>(string assetName) where T : Object;
        Task<IAsset> LoadAsync<T>(string assetName) where T : Object;
    }
}