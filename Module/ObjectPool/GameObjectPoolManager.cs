using Framework.Module.Resource;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework.Module.ObjectPool
{
    [Dependency(typeof(IResourceManager))]
    internal sealed class GameObjectPoolManager : Module, IGameObjectPoolManager
    {
        Dictionary<string, GameObjectPool> pools = new Dictionary<string, GameObjectPool>();
        GameObjectPoolPool cachePool = new GameObjectPoolPool();
        Queue<GameObjectPool> disposeQueue = new Queue<GameObjectPool>();
        bool disposeCurrent = true;
        float lastCheckTime = 0f;
        float checkTime = 20f;
        float sleepTime = 60f;

        GameObject cacheRoot;

        internal GameObjectPoolManager()
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

        public void SetCheckTime(float time)
        {
            if (time <= 0)
            {
                return;
            }

            this.checkTime = time;
        }

        public void SetSleepTime(float time)
        {
            if(time <= 0)
            {
                return;
            }

            this.sleepTime = time;
        }

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

        public async Task<GameObject> Pop(string gameObjectName)
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