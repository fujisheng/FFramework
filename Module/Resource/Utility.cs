using System;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Module.Resource
{
    public delegate Object LoadDelegate(string path, Type type);
    public delegate string GetPlatformDelegate();

    public static class Utility
    {
        public const string AssetBundles = "AssetBundles";
        public const string AssetsManifestAsset = "Assets/Sources/GameSetting/AssetBundleManifest.asset";
        public static bool assetBundleMode = true;
        public static LoadDelegate loadDelegate = null;
        public static GetPlatformDelegate getPlatformDelegate = null;

        public static string dataPath { get; set; }

        public static string GetPlatform()
        {
            return getPlatformDelegate != null
                ? getPlatformDelegate()
                : GetPlatformForAssetBundles(Application.platform);
        }

        private static string GetPlatformForAssetBundles(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                case RuntimePlatform.WebGLPlayer:
                    return "WebGL";
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    return "Windows";
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.OSXEditor:
                    return "OSX";
                default:
                    return null;
            }
        }

        public static string updatePath
        {
            get
            {
                return Path.Combine(Application.persistentDataPath, Path.Combine(AssetBundles, GetPlatform())) +
                       Path.DirectorySeparatorChar;
            }
        }

        public static string GetRelativePath4Update(string path)
        {
            return updatePath + path;
        }

        public static string GetWebUrlFromDataPath(string filename)
        {
            var path = Path.Combine(dataPath, Path.Combine(AssetBundles, GetPlatform())) + Path.DirectorySeparatorChar + filename;
#if UNITY_IOS || UNITY_EDITOR
            path = "file://" + path;
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            path = "file:///" + path;
#endif
            return path;
        }

        public static string GetWebUrlFromStreamingAssets(string filename)
        {
            var path = updatePath + filename;
            if (!File.Exists(path))
            {
                path = Application.streamingAssetsPath + "/" + filename;
            }
#if UNITY_IOS || UNITY_EDITOR
			path = "file://" + path;
#elif UNITY_STANDALONE_WIN
            path = "file:///" + path;
#endif
            return path;
        }
    }
}