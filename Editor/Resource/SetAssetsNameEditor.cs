﻿using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;

namespace Framework.Module.Resource.Editor
{
    public class SetAssetsNameEditor : AssetPostprocessor
    {
        static Dictionary<string, string> mapping = new Dictionary<string, string>();
        static readonly string dirPath = "Assets/Sources";
        static readonly List<string> ignoreExtensions = new List<string>
        {
            ".meta",
            ".DS_Store",
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
        //TODO 后续改成更改一个只变一个  同时如果删除了资源对应的也移除
        [MenuItem("Tools/Addressable/SetAddress")]
        public static void SetAddress()
        {
            mapping.Clear();
            GetPath(dirPath);
            foreach(var kv in mapping)
            {
                CreateAddressableAssetEntry(kv.Value, kv.Key);
            }
            Debug.Log("设置资源名字路径对应表成功！！！");
            //AssetDatabase.CreateAsset(assetsNameMapping, $"{outputPath}/AssetsNameMapping.asset");
            AssetDatabase.Refresh();
        }

        static void CreateAddressableAssetEntry(string path, string address, string groupName = "Default Local Group")
        {
#if UNITY_EDITOR
            AddressableAssetSettings settings;
            settings = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>("Assets/AddressableAssetsData/AddressableAssetSettings.asset");
            AddressableAssetGroup group = settings.FindGroup(groupName);
            if (group == null)
            {
                group = settings.CreateGroup(groupName, false, false, false, null, typeof(BundledAssetGroupSchema));
            }
            string assetGUID = AssetDatabase.AssetPathToGUID(path);
            AddressableAssetEntry entry = settings.CreateOrMoveEntry(assetGUID, group);
            entry.address = address;
#endif
        }

        static void GetPath(string dirPath)
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
                //foreach (var kv in replaceStr)
                //{
                //    fileName = fileName.Replace(kv.Key, kv.Value);
                //}

                if (mapping.ContainsKey(fileName))
                {
                    Debug.LogError($"已经包含这个名字的资源！！！请保证资源名字唯一  {fileName}");
                    return;
                }

                mapping.Add(fileName, filePath.Replace('\\', '/'));
            }

            if (Directory.GetDirectories(dirPath).Length > 0)
            {
                foreach (string dir in Directory.GetDirectories(dirPath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(dir);
                    //if (ignoreDir.Contains(directoryInfo.Name))
                    //{
                    //    continue;
                    //}
                    GetPath(dir);
                }
            }
        }
    }
}