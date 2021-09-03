using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using System.Linq;

namespace Framework.Service.Resource.Editor
{
    public class SetAssetsNameEditor : AssetPostprocessor
    {
        static Dictionary<string, string> Mapping = new Dictionary<string, string>();
        static FrameworkResourceSetting Setting;

        [MenuItem("Tools/Framework/CreateResourceSetting")]
        public static void CreateSetting()
        {
            if (!Directory.Exists("Assets/Editor Default Resources/"))
            {
                Directory.CreateDirectory("Assets/Editor Default Resources/");
            }

            var setting = ScriptableObject.CreateInstance<FrameworkResourceSetting>();
            AssetDatabase.CreateAsset(setting, "Assets/Editor Default Resources/FrameworkResourcesSetting.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        static void CheckSetting()
        {
            var setting = EditorGUIUtility.Load("FrameworkResourcesSetting.asset") as FrameworkResourceSetting;
            if (setting == null)
            {
                UnityEngine.Debug.LogWarning("FrameworkResourcesSetting is Empty, please create with menu [Tools/Framework/CreateResourceSetting]");
                return;
            }
            Setting = setting;
        }

        static void OnPostprocessAllAssets(string[] impertedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            CheckSetting();
            if(Setting == null)
            {
                return;
            }

            List<string> allPath = new List<string>();
            allPath.AddRange(impertedAssets);
            allPath.AddRange(deletedAssets);
            allPath.AddRange(movedAssets);
            allPath.AddRange(movedFromAssetPaths);

            bool isAssetChanged = false;
            foreach(var path in allPath)
            {
                if (path.StartsWith(Setting.sourcesPath))
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
        [MenuItem("Tools/Framework/SetResourcesAddress")]
        public static void SetAddress()
        {
            Mapping.Clear();
            GetPath(Setting.sourcesPath);
            foreach(var kv in Mapping)
            {
                string subPath = kv.Value.Substring(Setting.sourcesPath.Length);
                string[] names = subPath.Split('/');
                string groupName = "";
                for(int i = 0; i < names.Length - 1; i++)
                {
                    if (!string.IsNullOrEmpty(names[i]))
                    {
                        groupName += (names[i] + (i == names.Length - 2 ? "" : "_"));
                    }
                }
                string label = names[names.Length - 2];
                CreateAddressableAssetEntry(kv.Value, kv.Key, label, string.IsNullOrEmpty(groupName) ? Setting.defaultGroupName : groupName);
            }
            //AssetDatabase.CreateAsset(assetsNameMapping, $"{outputPath}/AssetsNameMapping.asset");
            AssetDatabase.Refresh();
        }

        static void CreateAddressableAssetEntry(string path, string address, string label, string groupName = "Default Local Group")
        {
            AddressableAssetSettings settings;
            settings = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>("Assets/AddressableAssetsData/AddressableAssetSettings.asset");
            AddressableAssetGroup group = settings.FindGroup(groupName);
            if (group == null)
            {
                group = settings.CreateGroup(groupName, false, false, false, null, typeof(BundledAssetGroupSchema));
            }
            string assetGUID = AssetDatabase.AssetPathToGUID(path);
            AddressableAssetEntry entry = settings.CreateOrMoveEntry(assetGUID, group);
            entry.SetAddress(address);
            var labels = settings.GetLabels();
            if(!labels.Contains(label))
            {
                settings.AddLabel(label);
            }
            entry.SetLabel(label, true);
        }

        static void GetPath(string dirPath)
        {
            foreach (string file in Directory.GetFiles(dirPath))
            {
                FileInfo fileInfo = new FileInfo(file);
                if (Setting.ignoreExtensions.Contains(fileInfo.Extension))
                {
                    continue;
                }

                string filePath = file;
                string fileName = Path.GetFileNameWithoutExtension(file);
                //foreach (var kv in replaceStr)
                //{
                //    fileName = fileName.Replace(kv.Key, kv.Value);
                //}

                if (Mapping.ContainsKey(fileName))
                {
                    UnityEngine.Debug.LogError($"已经包含这个名字的资源！！！请保证资源名字唯一  {fileName}");
                    return;
                }

                Mapping.Add(fileName, filePath.Replace('\\', '/'));
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