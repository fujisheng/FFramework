using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Framework.Module.Resource.Editor
{
    public static class BuildScript
    {
        public static string overloadedDevelopmentServerURL = "";

        public static void CopyAssetBundlesTo(string outputPath)
        {
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            var outputFolder = GetPlatformName();
            var source = Path.Combine(Path.Combine(Environment.CurrentDirectory, Utility.AssetBundles), outputFolder);
            if (!Directory.Exists(source))
            {
                Debug.Log("No assetBundle output folder, try to build the assetBundles first.");
            }
                
            var destination = Path.Combine(outputPath, outputFolder);
            if (Directory.Exists(destination))
            {
                FileUtil.DeleteFileOrDirectory(destination);
            }
                
            FileUtil.CopyFileOrDirectory(source, destination);
        }

        public static string GetPlatformName()
        {
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.Android:
                    return "Android";
                case BuildTarget.iOS:
                    return "iOS";
                case BuildTarget.WebGL:
                    return "WebGL";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "Windows";
                case BuildTarget.StandaloneOSX:
                    return "OSX";
                default:
                    return null;
            }
        }

        public static string CreateAssetBundleDirectory()
        {
            var outputPath = Path.Combine(Utility.AssetBundles, GetPlatformName());
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            } 

            return outputPath;
        }

        private static Dictionary<string, string> GetVersions(AssetBundleManifest manifest)
        {
            var items = manifest.GetAllAssetBundles();
            return items.ToDictionary(item => item, item => manifest.GetAssetBundleHash(item).ToString());
        }

        private static void LoadVersions(string versionsTxt, IDictionary<string, string> versions)
        {
            if (versions == null)
            {
                throw new ArgumentNullException("versions");
            }
               
            if (!File.Exists(versionsTxt))
            {
                return;
            }
                
            using (var s = new StreamReader(versionsTxt))
            {
                string line;
                while ((line = s.ReadLine()) != null)
                {
                    if (line == string.Empty)
                    {
                        continue;
                    }
                        
                    var fields = line.Split(':');
                    if (fields.Length > 1)
                    {
                        versions.Add(fields[0], fields[1]);
                    } 
                }
            }
        }

        private static void SaveVersions(string versionsTxt, Dictionary<string, string> versions)
        {
            if (File.Exists(versionsTxt))
            {
                File.Delete(versionsTxt);
            }
                
            using (var s = new StreamWriter(versionsTxt))
            {
                foreach (var item in versions)
                {
                    s.WriteLine(item.Key + ':' + item.Value);
                }
                    
                s.Flush();
                s.Close();
            }
        }

        public static void RemoveUnusedAssetBundleNames()
        {
            var manifest = GetManifest();
            var assetBundleNames = manifest.bundles;
            var variantNames = manifest.activeVariants;
            var dirs = manifest.dirs;

            List<string> usedBundles = new List<string>();
            List<string> usedDirs = new List<string>();
            List<string> usedVariants = new List<string>();

            for (int i = 0; i < manifest.assets.Length; i++)
            {
                var item = manifest.assets[i];
                var assetPath = dirs[item.dir] + "/" + item.name;
                if (System.IO.File.Exists(assetPath) && !string.IsNullOrEmpty(manifest.bundles[item.bundle]))
                {
                    var bundleIndex = usedBundles.FindIndex((string obj) =>
                    {
                        return obj.Equals(assetBundleNames[item.bundle]);
                    });

                    if (bundleIndex == -1)
                    {
                        usedBundles.Add(assetBundleNames[item.bundle]);
                        bundleIndex = usedBundles.Count - 1;
                    }

                    var dir = System.IO.Path.GetDirectoryName(assetPath).Replace("\\", "/");
                    var dirIndex = usedDirs.FindIndex(delegate (string obj) { return obj == dir; });
                    if (dirIndex == -1)
                    {
                        usedDirs.Add(dir);
                        dirIndex = usedDirs.Count - 1;
                    }

                    if (item.variant != -1)
                    {
                        var variantIndex = usedVariants.FindIndex(delegate (string obj) { return obj == dir; });
                        if (variantIndex == -1)
                        {
                            usedVariants.Add(variantNames[item.variant]);
                            variantIndex = usedVariants.Count - 1;
                        }

                        item.variant = variantIndex;
                    }

                    item.bundle = bundleIndex;
                    item.dir = dirIndex;
                }
                else
                {
                    ArrayUtility.RemoveAt(ref manifest.assets, i);
                    i--;
                }
            }

            manifest.dirs = usedDirs.ToArray();
            manifest.bundles = usedBundles.ToArray();
            EditorUtility.SetDirty(manifest);
        }

        public static void SetAssetBundleNameAndVariant(string assetPath, string bundleName, string variant, AssetsManifest manifestCache = null)
        {
            var manifest = manifestCache == null ? GetManifest() : manifestCache;
            var dir = Path.GetDirectoryName(assetPath).Replace("\\", "/");
            var dirs = manifest.dirs;
            var dirIndex = ArrayUtility.FindIndex(dirs, (string obj) => { return obj == dir; });

            if (dirIndex == -1)
            {
                ArrayUtility.Add(ref manifest.dirs, dir);
                dirIndex = manifest.dirs.Length - 1;
                dirs = manifest.dirs;
            }

            var assetBundleNames = manifest.bundles;
            var bundleIndex = ArrayUtility.FindIndex(assetBundleNames, (string obj) => { return obj == bundleName; });

            if (bundleIndex == -1)
            {
                ArrayUtility.Add(ref manifest.bundles, bundleName);
                assetBundleNames = manifest.bundles;
                bundleIndex = assetBundleNames.Length - 1;
            }

            var variantNames = manifest.activeVariants;
            var variantIndex = ArrayUtility.FindIndex(variantNames, (string obj) => { return obj == variant; });

            if (variantIndex == -1 && !string.IsNullOrEmpty(variant))
            {
                ArrayUtility.Add(ref manifest.activeVariants, variant);
                variantNames = manifest.activeVariants;
                variantIndex = variantNames.Length - 1;
            }

            var assets = manifest.assets;
            var assetIndex = ArrayUtility.FindIndex(assets, (AssetData obj) =>
            {
                var path = dirs[obj.dir] + "/" + obj.name;
                return path == assetPath;
            });

            if (assetIndex == -1)
            {
                var info = new AssetData();
                ArrayUtility.Add(ref manifest.assets, info);
                assetIndex = manifest.assets.Length - 1;
                assets = manifest.assets;
            }

            var asset = assets[assetIndex];
            asset.name = Path.GetFileName(assetPath);
            asset.bundle = bundleIndex;
            asset.variant = variantIndex;
            asset.dir = dirIndex;
            if (manifestCache == null)
            {
                EditorUtility.SetDirty(manifest);
                AssetDatabase.SaveAssets();
            }
        }

        public static void BuildManifest()
        {
            var manifest = GetManifest();
            var assetPath = AssetDatabase.GetAssetPath(manifest);
            var bundleName = Path.GetFileNameWithoutExtension(assetPath).ToLower();
            SetAssetBundleNameAndVariant(assetPath, bundleName, null);
        }

        public static void BuildAssetBundles()
        {
            var assetManifest = GetManifest();
            RemoveUnusedAssetBundleNames();

            var assets = assetManifest.assets;
            var assetBundleNames = assetManifest.bundles;
            var dirs = assetManifest.dirs;

            var map = new Dictionary<string, List<string>>();

            foreach (var item in assetManifest.bundles)
            {
                map[item] = new List<string>();
            }

            foreach (var item in assets)
            {
                var assetPath = dirs[item.dir] + "/" + item.name;
                map[assetBundleNames[item.bundle]].Add(assetPath);
            }

            List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
            foreach (var item in map)
            {
                builds.Add(new AssetBundleBuild()
                {
                    assetBundleName = item.Key,
                    assetNames = item.Value.ToArray()
                });
            }

            var outputPath = CreateAssetBundleDirectory();
            const BuildAssetBundleOptions options = BuildAssetBundleOptions.ChunkBasedCompression;
            var manifest = BuildPipeline.BuildAssetBundles(outputPath, builds.ToArray(), options, EditorUserBuildSettings.activeBuildTarget);
            var versionsTxt = outputPath + "/versions.txt";
            var versions = new Dictionary<string, string>();
            LoadVersions(versionsTxt, versions);
            var buildVersions = GetVersions(manifest);
            var updates = new List<string>();

            foreach (var item in buildVersions)
            {
                string hash;
                var isNew = true;
                if (versions.TryGetValue(item.Key, out hash))
                {
                    if (hash.Equals(item.Value))
                    {
                        isNew = false;
                    } 
                }
                    
                if (isNew)
                {
                    updates.Add(item.Key);
                }  
            }

            if (updates.Count > 0)
            {
                using (var s = new StreamWriter(File.Open(outputPath + "/updates.txt", FileMode.Append)))
                {
                    s.WriteLine(DateTime.Now.ToFileTime() + ":");
                    foreach (var item in updates)
                    {
                        s.WriteLine(item);
                    }
                       
                    s.Flush();
                    s.Close();
                }

                SaveVersions(versionsTxt, buildVersions);
            }
            else
            {
                Debug.Log("nothing to update.");
            }

            string[] ignoredFiles = { GetPlatformName(), "versions.txt", "updates.txt", "manifest" };

            var files = Directory.GetFiles(outputPath, "*", SearchOption.AllDirectories);

            var deletes = (from t in files
                           let file = t.Replace('\\', '/').Replace(outputPath.Replace('\\', '/') + '/', "")
                           where !file.EndsWith(".manifest", StringComparison.Ordinal) && !Array.Exists(ignoredFiles, s => s.Equals(file))
                           where !buildVersions.ContainsKey(file)
                           select t).ToList();

            foreach (var delete in deletes)
            {
                if (!File.Exists(delete))
                {
                    continue;
                }
                    
                File.Delete(delete);
                File.Delete(delete + ".manifest");
            }

            deletes.Clear();
        }

        private static T GetAsset<T>(string path) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
            }

            return asset;
        }

        public static Settings GetSettings()
        {
            const string path = "Assets/Scripts/Framework/Editor/Resource/Resources/Settings.asset";
            return GetAsset<Settings>(path);
        }

        public static AssetsManifest GetManifest()
        {
            return GetAsset<AssetsManifest>(Utility.AssetsManifestAsset);
        }
    }
}