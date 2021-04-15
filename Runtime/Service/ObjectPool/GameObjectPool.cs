using Cysharp.Threading.Tasks;
using FInject;
using Framework.Service.Resource;
using System;
using UnityEngine;

namespace Framework.Service.ObjectPool
{
    internal class GameObjectPool : AsyncObjectPool<GameObject>, IGameObjectPool
    {
        string gameObjectName;

        [Inject]
        public IResourceLoader resourceLoader { set; private get; }
        public override int Size => 10;

        /// <summary>
        /// 构造方法 采用默认的资源加载器
        /// </summary>
        public GameObjectPool()
        {
        }

        /// <summary>
        /// 构造方法 采用默认的资源加载器
        /// </summary>
        /// <param name="gameObjectName">gameObject的名字</param>
        public GameObjectPool(string gameObjectName)
        {
            this.gameObjectName = gameObjectName;
        }

        /// <summary>
        /// 设置GameObject的名字
        /// </summary>
        /// <param name="gameObjectName">gameObject的名字</param>
        public void SetGameObjectName(string gameObjectName)
        {
            if(Count != 0)
            {
                return;
            }
            this.gameObjectName = gameObjectName;
        }

        /// <summary>
        /// 创建一个GameObject
        /// </summary>
        /// <returns></returns>
        protected override async UniTask<GameObject> New()
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

        /// <summary>
        /// 释放这个池
        /// </summary>
        public override void Release()
        {
            while (true)
            {
                if (pool.Count <= 0)
                {
                    break;
                }
                resourceLoader.ReleaseInstance(pool.Pop());
            }
            resourceLoader.Release();
        }
    }
}