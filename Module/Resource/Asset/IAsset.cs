using System;
using Object = UnityEngine.Object;

namespace Framework.Module.Resource
{
    public interface IAsset : IReference
    {
        Type AssetType { get; }
        string Name { get; }
        LoadState LoadState { get; }
        bool IsDone { get; }
        float Progress { get; }
        string Error { get; }
        string Text { get; }
        byte[] Bytes { get; }
        Object asset { get; }
        void Require(Object obj);
        void Dequire(Object obj);
        Action<IAsset> Completed { get; set; }
    }
}