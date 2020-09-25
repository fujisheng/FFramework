﻿using System.Threading.Tasks;
using UnityEngine;

namespace Framework.Module.Resource
{
    public interface IResourceLoader
    {
        Task<T> GetAsync<T>(string assetName) where T : Object;
        T Get<T>(string assetName) where T : Object;
        GameObject Instantiate(string assetName);
        Task<GameObject> InstantiateAsync(string assetName);
        void DestroyGameObject(GameObject gameObject);
        void Release();
    }
}