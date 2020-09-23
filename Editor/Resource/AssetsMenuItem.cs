using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Framework.Module.Resource.Editor
{
    public static class AssetsMenuItem
    {
        public static string assetRootPath;

        [InitializeOnLoadMethod]
        private static void OnInitialize()
        {
            EditorUtility.ClearProgressBar();
            var settings = BuildScript.GetSettings(); 
            Utility.dataPath = System.Environment.CurrentDirectory;
            Utility.assetBundleMode = settings.runtimeMode;
            Utility.getPlatformDelegate = BuildScript.GetPlatformName;
            Utility.loadDelegate = AssetDatabase.LoadAssetAtPath;
        }

        public static string TrimedAssetBundleName(string assetBundleName)
        {
            if (string.IsNullOrEmpty(assetRootPath))
            {
                return assetBundleName;
            }
            return assetBundleName.Replace(assetRootPath, "");
        }

        [MenuItem("Tools/AssetBundles/拷贝到StreamingAssets")]
        private static void CopyAssetBundles()
        {
            BuildScript.CopyAssetBundlesTo(Path.Combine(Application.streamingAssetsPath, Utility.AssetBundles));
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/AssetBundles/生成Manifest")]
        private static void CreateManifest()
        {
            var settings = BuildScript.GetSettings();
            assetRootPath = settings.assetRootPath; 
            var assetsManifest = BuildScript.GetManifest();
            List<Object> assets = new List<Object>();
            GetAssets(assetRootPath, assets);
            for (var i = 0; i < assets.Count; i++)
            {
                var asset = assets[i];
                var path = AssetDatabase.GetAssetPath(asset);
                if (Directory.Exists(path) || path.EndsWith(".cs", System.StringComparison.CurrentCulture))
                {
                    continue;
                }
                if (EditorUtility.DisplayCancelableProgressBar("标记资源中", path, i * 1f / assets.Count))
                {
                    break;
                }
                var assetBundleName = TrimedAssetBundleName(Path.GetDirectoryName(path).Replace("\\", "/")) + "_g";
                assetBundleName = assetBundleName.StartsWith("/") ? assetBundleName.Substring(1) : assetBundleName;
                BuildScript.SetAssetBundleNameAndVariant(path, assetBundleName.ToLower(), null, assetsManifest);
            }
            EditorUtility.SetDirty(assetsManifest);
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
        }

        static void GetAssets(string dirPath, List<Object> allAssets)
        {
            foreach (string file in Directory.GetFiles(dirPath))
            {
                FileInfo fileInfo = new FileInfo(file);
                if(fileInfo.Extension == ".meta")
                {
                    continue;
                }
                string filePath = file.Replace('\\', '/');
                allAssets.Add(AssetDatabase.LoadAssetAtPath<Object>(filePath));
            }

            if (Directory.GetDirectories(dirPath).Length > 0)
            {
                foreach (string dir in Directory.GetDirectories(dirPath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(dir);
                    GetAssets(dir, allAssets);
                }
            }
        }

        [MenuItem("Tools/AssetBundles/生成資源包")]
        private static void BuildAssetBundles()
        {
            BuildScript.BuildManifest();
            BuildScript.BuildAssetBundles();
        }
    }
}