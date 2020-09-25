using UnityEngine;
using Framework.Module.Resource;

namespace Framework.Module.ObjectPool
{
    internal interface IGameObjectPool
    {
        void SetGameObjectName(string gameObjectName);
        void SetResourceLoader(IResourceLoader resourceLoader);
    }
}

