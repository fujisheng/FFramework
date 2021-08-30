using Cysharp.Threading.Tasks;
using Framework.Service.Resource;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework.Service.ObjectPool
{
    /// <summary>
    /// 管理所有的GameObject池
    /// </summary>
    [Dependencies(typeof(IResourceService))]
    internal sealed class GameObjectPoolService : Service, IGameObjectPoolService
    {
        Dictionary<string, GameObjectPool> pools = new Dictionary<string, GameObjectPool>();
        GameObjectPoolPool cachePool = new GameObjectPoolPool();
        Queue<GameObjectPool> disposeQueue = new Queue<GameObjectPool>();
        bool disposeCurrent = true;
        float lastCheckTime = 0f;
        float checkTime = 20f;
        float sleepTime = 60f;

        GameObject cacheRoot;

        internal GameObjectPoolService()
        {
            cacheRoot = new GameObject("[GameObjectPool]");
            Object.DontDestroyOnLoad(cacheRoot);
        }

        internal override void OnLateUpdate()
        {
            if (Time.realtimeSinceStartup - lastCheckTime >= checkTime)
            {
                CheckSleep();
                lastCheckTime = Time.realtimeSinceStartup;
            }

            DisposeQueue();
        }

        /// <summary>
        /// 设置检查时间
        /// </summary>
        /// <param name="time">每隔多久检查是否有休眠的池子</param>
        public void SetCheckTime(float time)
        {
            if (time <= 0)
            {
                return;
            }

            this.checkTime = time;
        }

        /// <summary>
        /// 设置休眠时间
        /// </summary>
        /// <param name="time">池子的休眠时间</param>
        public void SetSleepTime(float time)
        {
            if(time <= 0)
            {
                return;
            }

            this.sleepTime = time;
        }

        /// <summary>
        /// 检查池子是否处于休眠状态
        /// </summary>
        void CheckSleep()
        {
            List<string> removeKeys = new List<string>();
            foreach(var key in pools.Keys)
            {
                GameObjectPool pool = pools[key];
                if(Time.realtimeSinceStartup - pool.LastUseTime >= sleepTime)
                {
                    disposeQueue.Enqueue(pool);
                    removeKeys.Add(key);
                }
            }

            foreach(var key in removeKeys)
            {
                pools.Remove(key);
            }
        }

        /// <summary>
        /// 释放队列
        /// </summary>
        async void DisposeQueue()
        {
            if (disposeCurrent == false)
            {
                return;
            }

            if (disposeQueue.Count <= 0)
            {
                return;
            }

            disposeCurrent = false;
            disposeQueue.Dequeue().Release();
            await Task.Yield();
            disposeCurrent = true;
        }

        /// <summary>
        /// 压入某个GameObject
        /// </summary>
        /// <param name="gameObjectName">名字</param>
        /// <param name="gameObject">对应的GameObject</param>
        public void Push(string gameObjectName, GameObject gameObject)
        {
            gameObject.SetActive(false);
            gameObject.transform.SetParent(cacheRoot.transform);
            if (pools.ContainsKey(gameObjectName))
            {
                pools[gameObjectName].Push(gameObject);
                return;
            }

            GameObjectPool pool = cachePool.Pop();
            pool.SetGameObjectName(gameObjectName);
            pool.Push(gameObject);
            pools.Add(gameObjectName, pool);
        }

        /// <summary>
        /// 弹出某个名字的GameObject
        /// </summary>
        /// <param name="gameObjectName">名字</param>
        /// <returns>GameObject</returns>
        public async UniTask<GameObject> Pop(string gameObjectName)
        {
            if (pools.ContainsKey(gameObjectName))
            {
                GameObjectPool pool = pools[gameObjectName];
                GameObject gameObject = await pool.Pop();
                if (pool.Count == 0)
                {
                    cachePool.Push(pools[gameObjectName]);
                    pools.Remove(gameObjectName);
                }
                return gameObject;
            }

            GameObjectPool newPool = cachePool.Pop();
            newPool.SetGameObjectName(gameObjectName);
            return await newPool.Pop();
        }

        /// <summary>
        /// 释放所有的池子
        /// </summary>
        public void Release()
        {
            cachePool.Dispose();

            string[] removeKeys = new string[pools.Count];
            int index = 0;
            foreach(var kv in pools)
            {
                string key = kv.Key;
                GameObjectPool pool = kv.Value;
                disposeQueue.Enqueue(pool);
                removeKeys[index] = key;
                index++;
            }
            
            foreach(var key in removeKeys)
            {
                pools.Remove(key);
            }
        }
    }
}