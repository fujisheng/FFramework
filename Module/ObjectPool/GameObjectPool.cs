using Framework.Module.Resource;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework.Module.ObjectPool
{
    public class GameObjectPool : AsyncObjectPool<GameObject>, IGameObjectPool
    {
        string gameObjectName;
        IResourceManager resManager;
        public override int Size => 10;

        public GameObjectPool()
        {
            resManager = ModuleManager.GetModule<IResourceManager>();
        }

        public GameObjectPool(string gameObjectName)
        {
            this.gameObjectName = gameObjectName;
            resManager = ModuleManager.GetModule<IResourceManager>();
        }

        public void SetGameObjectName(string gameObjectName)
        {
            if(Count != 0)
            {
                return;
            }
            this.gameObjectName = gameObjectName;
        }

        public void SetResManager(IResourceManager resManager)
        {
            this.resManager = resManager;
        }


        public override async Task<GameObject> New()
        {
            if (string.IsNullOrEmpty(gameObjectName))
            {
                Debug.LogError("还没有为当前GameObject设置名字");
                return null;
            }
            if (resManager == null)
            {
                Debug.LogError("当前resManager为空！！！");
            }
            IAsset asset = await resManager.LoadAsync<GameObject>(gameObjectName);
            GameObject gameObjectR = UnityEngine.Object.Instantiate(asset.asset as GameObject);
            gameObjectR.SetActive(true);
            asset.Require(gameObjectR);
            return gameObjectR;
        }

        public override void Dispose()
        {
            while (true)
            {
                if (pool.Count <= 0)
                {
                    break;
                }

                GameObject.Destroy(pool.Pop());
            }
        }
    }
}