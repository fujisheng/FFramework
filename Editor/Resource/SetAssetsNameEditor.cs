using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Framework.Module.Resource.Editor
{
    public class SetAssetsNameEditor : AssetPostprocessor
    {
        static readonly string dirPath = "Assets/Resources";
        static readonly string outputPath = "Assets/Resources/GameSetting";
        static readonly List<string> ignoreExtensions = new List<string>
        {
            ".meta",
            ".DS_Store",
        };
        static readonly List<string> ignoreDir = new List<string>
        {

        };

        static readonly Dictionary<string, string> replaceStr = new Dictionary<string, string>
        {

        };

        static void OnPostprocessAllAssets(string[] impertedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            List<string> allPath = new List<string>();
            allPath.AddRange(impertedAssets);
            allPath.AddRange(deletedAssets);
            allPath.AddRange(movedAssets);
            allPath.AddRange(movedFromAssetPaths);

            bool isAssetChanged = false;
            foreach(var path in allPath)
            {
                if (path.StartsWith(dirPath))
                {
                    isAssetChanged = true;
                    break;
                }
            }

            if (isAssetChanged)
            {
                SetAddress();
            }
        }

        [MenuItem("Tools/AssetBundles/设置资源名字路径对应表")]
        public static void SetAddress()
        {
            AssetsNameMapping assetsNameMapping = ScriptableObject.CreateInstance<AssetsNameMapping>();
            GetPath(dirPath, assetsNameMapping);
            Debug.Log("设置资源名字路径对应表成功！！！");
            AssetDatabase.CreateAsset(assetsNameMapping, $"{outputPath}/AssetsNameMapping.asset");
            AssetDatabase.Refresh();
        }

        static void GetPath(string dirPath, AssetsNameMapping mapping)
        {
            foreach (string file in Directory.GetFiles(dirPath))
            {
                FileInfo fileInfo = new FileInfo(file);
                if (ignoreExtensions.Contains(fileInfo.Extension))
                {
                    continue;
                }

                string filePath = file;
                string fileName = Path.GetFileNameWithoutExtension(file);
                foreach (var kv in replaceStr)
                {
                    fileName = fileName.Replace(kv.Key, kv.Value);
                }

                if (mapping.mapping.ContainsKey(fileName))
                {
                    Debug.LogError($"已经包含这个名字的资源！！！请保证资源名字唯一  {fileName}");
                    return;
                }
                
                mapping.mapping.Add(fileName, filePath.Replace('\\', '/'));
            }

            if (Directory.GetDirectories(dirPath).Length > 0)
            {
                foreach (string dir in Directory.GetDirectories(dirPath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(dir);
                    if (ignoreDir.Contains(directoryInfo.Name))
                    {
                        continue;
                    }
                    GetPath(dir, mapping);
                }
            }
        }
    }
}