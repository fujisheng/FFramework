using UnityEngine;
using Framework.Module.Resource;

namespace Framework.Module.ObjectPool
{
    public interface IGameObjectPool : IObjectPool<GameObject>
    {
        void SetGameObjectName(string gameObjectName);
        void SetResourceLoader(IResourceLoader resourceLoader);
    }
}

