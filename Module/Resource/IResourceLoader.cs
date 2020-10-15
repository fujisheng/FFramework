using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;

namespace Framework.Module.Resource
{
    public interface IResourceLoader
    {
        Task<T> GetAsync<T>(string assetName) where T : Object;
        Task<IList<T>> GetAllAsync<T>(string lable) where T : Object;
        Task<IList<T>> GetAllAsync<T>(IList<string> labelOrNames) where T : Object;
        Task Perload<T>(string assetName) where T : Object;
        Task PerloadAll<T>(string label) where T : Object;
        Task PerloadAll<T>(IList<string> labelOrNames) where T : Object;
        T Get<T>(string assetName) where T : Object;
        Task<GameObject> InstantiateAsync(string assetName);
        void ReleaseInstance(GameObject gameObject);

        /// <summary>
        /// 释放这个Loader 请在释放后将其置空
        /// </summary>
        void Release();
    }
}