using Framework.Module.Resource;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework.Module.ObjectPool
{
    public class GameObjectPool : AsyncObjectPool<GameObject>, IGameObjectPool
    {
        string gameObjectName;
        IResourceLoader resourceLoader;
        public override int Size => 10;

        public GameObjectPool()
        {
            resourceLoader = new ResourceLoader();
        }

        public GameObjectPool(string gameObjectName)
        {
            this.gameObjectName = gameObjectName;
            resourceLoader = new ResourceLoader();
        }

        public void SetGameObjectName(string gameObjectName)
        {
            if(Count != 0)
            {
                return;
            }
            this.gameObjectName = gameObjectName;
        }

        public void SetResourceLoader(IResourceLoader  resourceLoader)
        {
            this.resourceLoader = resourceLoader;
        }


        public override async Task<GameObject> New()
        {
            if (string.IsNullOrEmpty(gameObjectName))
            {
                throw new Exception("gameObject name is empty, need set gameObject name first");
            }
            if (resourceLoader == null)
            {
                throw new Exception("resourceLoader is empty, need set resourceLoader first");
            }
            var gameObject = await resourceLoader.InstantiateAsync(gameObjectName);
            gameObject.SetActive(true);
            return gameObject;
        }

        public override void Dispose()
        {
            while (true)
            {
                if (pool.Count <= 0)
                {
                    break;
                }
                resourceLoader.DestroyGameObject(pool.Pop());
            }
            resourceLoader.Release();
        }
    }
}