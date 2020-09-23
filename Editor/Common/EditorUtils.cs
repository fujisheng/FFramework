using System;
using System.IO;
using System.Text;
using UnityEditor;
//using UnityEditor.AddressableAssets.Settings;
//using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

public class EditorUtils
{
    public static void CreateAddressableAssetEntry(string path, string address, string groupName = "Default Local Group")
    {
        //AddressableAssetSettings settings;
        //settings = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>("Assets/AddressableAssetsData/AddressableAssetSettings.asset");
        //AddressableAssetGroup group = settings.FindGroup(groupName);
        //if (group == null)
        //{
        //    group = settings.CreateGroup(groupName, false, false, false, null, typeof(BundledAssetGroupSchema));
        //}
        //string assetGUID = AssetDatabase.AssetPathToGUID(path);
        //AddressableAssetEntry entry = settings.CreateOrMoveEntry(assetGUID, group);
        //entry.address = address;
    }

    public static string GetAddressWithPath(string path)
    {
        //AddressableAssetSettings settings;
        //settings = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>("Assets/AddressableAssetsData/AddressableAssetSettings.asset");
        //string assetGUID = AssetDatabase.AssetPathToGUID(path);
        //return settings.FindAssetEntry(assetGUID).address;
        return null;
    }

    public static void SaveStringToFile(string str, string fileName, string suffixName, string path)
    {
        string finalRootPath = path;
        string finalPath = $"{finalRootPath}/{fileName}.{suffixName}";
        if (!Directory.Exists(finalRootPath))
        {
            Directory.CreateDirectory(finalRootPath);
        }

        if (Directory.Exists(finalPath))
        {
            Directory.Delete(finalPath);
        }

        FileStream file = new FileStream(finalPath, FileMode.Create);
        byte[] bts = System.Text.Encoding.UTF8.GetBytes(str);
        file.Write(bts, 0, bts.Length);
        if (file != null)
        {
            file.Close();
        }
    }

    public static void SaveBytesToFile(byte[] bytes, string fileName, string suffixName, string path)
    {
        string finalRootPath = path;
        string finalPath = $"{finalRootPath}/{fileName}.{suffixName}";
        if (!Directory.Exists(finalRootPath))
        {
            Directory.CreateDirectory(finalRootPath);
        }

        if (Directory.Exists(finalPath))
        {
            Directory.Delete(finalPath);
        }

        using (FileStream fs = new FileStream(finalPath, FileMode.CreateNew, FileAccess.ReadWrite))
        {
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                writer.Write(bytes, 0, bytes.Length);
            }
        }
    }
}
