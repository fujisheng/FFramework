using Object = UnityEngine.Object;

namespace Framework.Module.Resource
{
    internal class InstantiableAsset : Asset
    {
        internal InstantiableAsset(Object obj) : base(obj) { }
        public override void Release()
        {
            UnityEngine.Debug.Log($"Asset[{asset.name}] Release");
            Object.Destroy(this.asset);
            this.asset = null;
        }
    }
}