using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Module.Resource
{
    public interface IResourceLoader
    {
        UniTask<T> GetAsync<T>(string assetName) where T : Object;
        UniTask<IList<T>> GetAllAsync<T>(string lable) where T : Object;
        UniTask<IList<T>> GetAllAsync<T>(IList<string> labelOrNames) where T : Object;
        UniTask Perload<T>(string assetName) where T : Object;
        UniTask PerloadAll<T>(string label) where T : Object;
        UniTask PerloadAll<T>(IList<string> labelOrNames) where T : Object;
        T Get<T>(string assetName) where T : Object;
        UniTask<GameObject> InstantiateAsync(string assetName, Vector3 position = default, Quaternion rotation = default, Transform parent = null, bool trackHandle = true);
        void ReleaseInstance(GameObject gameObject);

        /// <summary>
        /// 释放这个Loader 请在释放后将其置空
        /// </summary>
        void Release();
    }
}