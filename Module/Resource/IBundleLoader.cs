using System;

namespace Framework.Module.Resource
{
    public interface IBundleLoader
    {
        string[] ActiveVariants { get; set; }
        OverrideDataPathDelegate OverrideBaseDownloadingUrl { get; set; }
        string[] GetAllDependencies(string bundle);
        void Initialize(string path, string platform, Action onSuccess, Action<string> onError);
        Bundle Load(string assetBundleName);
        Bundle LoadAsync(string assetBundleName);
        void Unload(Bundle bundle);
        void Unload(string assetBundleName);
        void Update();
    }
}