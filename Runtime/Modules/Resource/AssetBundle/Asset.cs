using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Module.Resource
{
    internal class Asset : IReference
    {
        protected Object asset;
        protected string assetName;
        internal Asset(Object obj)
        {
            this.asset = obj;
            this.assetName = obj.name;
        }

        public virtual void Release()
        {
            // AssetBundle资源由Bundle.Unload管理，不要在这里调用DestroyImmediate或UnloadAsset
            // 只断开引用，让GC和Bundle.Unload处理实际卸载
#if DEBUG_RESOURCE
            UnityEngine.Debug.Log($"Asset[{assetName}] Release");
#endif
            ResourceLeakDetector.RecordRelease(this);
            asset = null;
            assetName = string.Empty;
        }

        internal bool Is<T>() where T : Object
        {
            return asset is T;
        }

        internal T As<T>() where T : Object
        {
            Utility.Assert.IfNot<T>(asset, new System.Exception($"{asset} is not {typeof(T)}"));
            return asset as T;
        }

        public override string ToString()
        {
            return $"asset:{asset}";
        }
    }
}