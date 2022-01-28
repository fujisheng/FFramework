using System.IO;

namespace Framework.Editor
{
    public static partial class Utility
    {
        public static class File
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

            /// <summary>
            /// 保存字符串到文件
            /// </summary>
            /// <param name="str">字符串</param>
            /// <param name="fileName">文件名</param>
            /// <param name="suffixName">后缀名</param>
            /// <param name="path">路径</param>
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

            /// <summary>
            /// 保存bytes到文件
            /// </summary>
            /// <param name="bytes">字节数组</param>
            /// <param name="fileName">文件名</param>
            /// <param name="suffixName">后缀名</param>
            /// <param name="path">路径</param>
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
    }
}