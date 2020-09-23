using System;
using System.Collections.Generic;

namespace Framework.Module.Resource
{
    public interface IAssetLoader
    {
        Dictionary<string ,int> BundleAssets { get; }
        void Initialize(Action onSuccess, Action<string> onError, IBundleLoader bundleLoader);
        string[] GetAllDependencies(string path);
        Asset LoadScene(string path, bool async, bool addictive);
        void UnloadScene(string path);
        Asset Load(string path, Type type);
        Asset LoadAsync(string path, Type type);
        void Unload(Asset asset);
        void Update();
    }
}