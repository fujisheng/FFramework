using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Framework.Module.Resource
{
    public class AssetLoader : IAssetLoader
    {
        string[] _bundles = new string[0];
        readonly List<Asset> _assets = new List<Asset>();
        readonly List<Asset> _unusedAssets = new List<Asset>();
        public Dictionary<string, int> BundleAssets{ get; private set; }
        string updatePath { get; set; }
        IBundleLoader bundleLoader;

        /// <summary>
        /// 初始化loader
        /// </summary>
        /// <param name="onSuccess">初始化成功的回调</param>
        /// <param name="onError">初始化失败的回调</param>
        /// <param name="bundleLoader">bundleLoader</param>
        public void Initialize(Action onSuccess, Action<string> onError, IBundleLoader bundleLoader)
        {
            this.bundleLoader = bundleLoader;

            if (string.IsNullOrEmpty(Utility.dataPath))
            {
                Utility.dataPath = Application.streamingAssetsPath;
            }

            Log(string.Format("Init->assetBundleMode {0} | dataPath {1}", Utility.assetBundleMode, Utility.dataPath));

            if (Utility.assetBundleMode)
            {
                updatePath = Utility.updatePath;
                var platform = Utility.GetPlatform();
                var path = Path.Combine(Utility.dataPath, Path.Combine(Utility.AssetBundles, platform)) +
                           Path.DirectorySeparatorChar;
                bundleLoader.OverrideBaseDownloadingUrl += Bundles_overrideBaseDownloadingURL;
                bundleLoader.Initialize(path, platform, () =>
                {
                    var asset = LoadAsync(Utility.AssetsManifestAsset, typeof(AssetsManifest));
                    asset.Completed += obj =>
                    {
                        var manifest = obj.asset as AssetsManifest;
                        if (manifest == null)
                        {
                            onError?.Invoke("manifest == null");
                            return;
                        }
                            
                        bundleLoader.ActiveVariants = manifest.activeVariants;
                        _bundles = manifest.bundles;
                        var dirs = manifest.dirs;
                        BundleAssets = new Dictionary<string, int>(manifest.assets.Length);
                        for (int i = 0, max = manifest.assets.Length; i < max; i++)
                        {
                            var item = manifest.assets[i];
                            BundleAssets[string.Format("{0}/{1}", dirs[item.dir], item.name)] = item.bundle;
                        }

                        onSuccess?.Invoke();
                        (obj as IReference).Release();
                    };
                }, onError);
            }
            else
            {
                onSuccess?.Invoke();
            }
        }

        /// <summary>
        /// 获取asset依赖的bundle
        /// </summary>
        /// <param name="path">asset的路径</param>
        /// <returns></returns>
        public string[] GetAllDependencies(string path)
        {
            string assetBundleName;
            return GetAssetBundleName(path, out assetBundleName) ? bundleLoader.GetAllDependencies(assetBundleName) : null;
        }

        /// <summary>
        /// 加载一个场景
        /// </summary>
        /// <param name="path">场景的路径</param>
        /// <param name="async">是否异步加载</param>
        /// <param name="addictive"></param>
        /// <returns></returns>
        public Asset LoadScene(string path, bool async, bool addictive)
        {
            var asset = async ? new SceneAssetAsync(path, addictive, bundleLoader) : new SceneAsset(path, addictive, bundleLoader);
            GetAssetBundleName(path, out asset.assetBundleName);
            asset.Load();
            asset.Retain();
            _assets.Add(asset);
            return asset;
        }

        /// <summary>
        /// 卸载一个场景
        /// </summary>
        /// <param name="path">要卸载的场景的路径</param>
        public void UnloadScene(string path)
        {
            for (int i = 0, max = _assets.Count; i < max; i++)
            {
                var item = _assets[i];
                if (!item.Name.Equals(path))
                {
                    continue;
                }  
                Unload(item);
                break;
            }
        }

        /// <summary>
        /// 同步加载一个资源
        /// </summary>
        /// <param name="path">要加载的资源的路径</param>
        /// <param name="type">要加载的资源的类型</param>
        /// <returns></returns>
        public Asset Load(string path, Type type)
        {
            return Load(path, type, false);
        }

        /// <summary>
        /// 异步加载一个资源
        /// </summary>
        /// <param name="path">要加载的资源的路径</param>
        /// <param name="type">要加载的资源的类型</param>
        /// <returns></returns>
        public Asset LoadAsync(string path, Type type)
        {
            return Load(path, type, true);
        }

        /// <summary>
        /// 卸载一个资源
        /// </summary>
        /// <param name="asset">要卸载的资源</param>
        public void Unload(Asset asset)
        {
            asset.Release();
            for (var i = 0; i < _unusedAssets.Count; i++)
            {
                var item = _unusedAssets[i];
                if (!item.Name.Equals(asset.Name))
                {
                    continue;
                } 
                item.Unload();
                _unusedAssets.RemoveAt(i);
                return;
            }
        }

        public void Update()
        {
            for (var i = 0; i < _assets.Count; i++)
            {
                var item = _assets[i];
                if (item.Update() || !(item as IReference).IsUnused)
                {
                    continue;
                } 
                _unusedAssets.Add(item);
                _assets.RemoveAt(i);
                i--;
            }

            for (var i = 0; i < _unusedAssets.Count; i++)
            {
                var item = _unusedAssets[i];
                item.Unload();
                Log("Unload->" + item.Name);
            }

            _unusedAssets.Clear();
            bundleLoader?.Update();
        }

        /// <summary>
        /// 加载一个资源
        /// </summary>
        /// <param name="path">要加载的资源的路径</param>
        /// <param name="type">要加载的资源的类型</param>
        /// <param name="async">是否异步加载</param>
        /// <returns></returns>
        Asset Load(string path, Type type, bool async)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("invalid path");
                return null;
            }

            for (int i = 0, max = _assets.Count; i < max; i++)
            {
                var item = _assets[i];
                if (!item.Name.Equals(path))
                {
                    continue;
                }   
                item.Retain();
                return item;
            }

            string assetBundleName;
            Asset asset;
            if (GetAssetBundleName(path, out assetBundleName))
            {
                asset = async ? new BundleAssetAsync(assetBundleName, bundleLoader) : new BundleAsset(assetBundleName, bundleLoader);
            }
            else
            {
                if (path.StartsWith("http://", StringComparison.Ordinal) ||
                    path.StartsWith("https://", StringComparison.Ordinal) ||
                    path.StartsWith("file://", StringComparison.Ordinal) ||
                    path.StartsWith("ftp://", StringComparison.Ordinal) ||
                    path.StartsWith("jar:file://", StringComparison.Ordinal))
                {
                    asset = new WebAsset();
                }
                else
                {
                    asset = new Asset();
                }  
            }

            asset.Name = path;
            asset.AssetType = type;
            _assets.Add(asset);
            asset.Load();
            asset.Retain();

            Log(string.Format("Load->{0}|{1}", path, assetBundleName));
            return asset;
        }

        /// <summary>
        /// 获取资源的bundle名
        /// </summary>
        /// <param name="path">资源的路径</param>
        /// <param name="assetBundleName">out资源bundle名</param>
        /// <returns></returns>
        bool GetAssetBundleName(string path, out string assetBundleName)
        {
            if (path.Equals(Utility.AssetsManifestAsset))
            {
                assetBundleName = Path.GetFileNameWithoutExtension(path).ToLower();
                return true;
            }

            assetBundleName = null;
            int bundle;
            if (!BundleAssets.TryGetValue(path, out bundle))
            {
                return false;
            }
            assetBundleName = _bundles[bundle];
            return true;
        }

        /// <summary>
        /// 获取bundle的下载链接
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        string Bundles_overrideBaseDownloadingURL(string bundleName)
        {
            return !File.Exists(Path.Combine(updatePath, bundleName)) ? null : updatePath;
        }

        [Conditional("LOG_ENABLE")]
        void Log(string s)
        {
            Debug.Log(string.Format("[Assets]{0}", s));
        }
    }
}