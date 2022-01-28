using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Framework.Service.Resource.Editor
{
    public class AssetBundleTools : UnityEditor.Editor
    {
        static readonly string dataPath = "Assets/Data";

        [MenuItem("Tools/BuildAssets")]
        public static void Build()
        {
            var builds = new List<AssetBundleBuild>();
            CreateBuilds(dataPath, builds);
            BuildPipeline.BuildAssetBundles("Assets/AssetsBundle", builds.ToArray(), BuildAssetBundleOptions.DisableWriteTypeTree, BuildTarget.Android);
            UnityEngine.Debug.Log("通过文件夹构建Bundle完成");
        }

        static void CreateBuilds(string path, List<AssetBundleBuild> result)
        {
            var files = Directory.GetFiles(path).Where(item => !item.EndsWith(".meta"));

            if (files.Count() > 0)
            {
                string bundleName = path.Replace($"{dataPath}\\", "");
                var build = new AssetBundleBuild() { assetBundleName = bundleName, assetNames = new string[files.Count()] };
                Directory.GetFiles(path)
                    .Where(item => !item.EndsWith(".meta")).ToList()
                    .ForEach(item => ArrayUtility.Add(ref build.assetNames, item.Replace("\\", "/")));// Path.GetFileName(item)));
                result.Add(build);
            }

            Directory.GetDirectories(path).ToList().ForEach(item => CreateBuilds(item, result));
        }
    }
}
