using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Service.Resource
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
            if (asset is GameObject)
            {
                Object.DestroyImmediate(asset, true);
            }
            else
            {
                Resources.UnloadAsset(asset);
            }
            UnityEngine.Debug.Log($"Asset[{assetName}] Release");

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