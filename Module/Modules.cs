using Framework.Module.Audio;
using Framework.Module.Database;
using Framework.Module.ObjectPool;
using Framework.Module.Resource;
using Framework.Module.Unity.Loop;
using Framework.Module.Unity.Threading;

namespace Framework.Module
{
    public static class Modules
    {
        public static IResourceManager Resource
        {
            get { return ModuleManager.Instance.GetModule<IResourceManager>(); }
        }

        public static ILoopManager Loop
        {
            get { return ModuleManager.Instance.GetModule<ILoopManager>(); }
        }

        public static IDatabaseManager Database
        {
            get { return ModuleManager.Instance.GetModule<IDatabaseManager>(); }
        }

        public static IAudioManager Audio
        {
            get { return ModuleManager.Instance.GetModule<IAudioManager>(); }
        }

        //public static ThreadManager Thread
        //{
        //    get { return ModuleManager.Instance.GetModule<ThreadManager>(); }
        //}

        public static IGameObjectPoolManager GameObjectPool
        {
            get { return ModuleManager.Instance.GetModule<IGameObjectPoolManager>(); }
        }
    }
}
