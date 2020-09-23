using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Editor
{
    [InitializeOnLoad]
    public class UIFontReplaceEditor : UnityEditor.Editor
    {

        static Queue<string> replaceQueue = new Queue<string>();
        static readonly string rootPath = "Assets/GameAsset";
        static readonly string fontPath = "Assets/Font/Lite";
        static bool replaceNowComplete = true;
        static int totalNumber = 0;
        static readonly List<string> ignoreDir = new List<string>
        {

        };

        static readonly Dictionary<string, string> replaceMapping = new Dictionary<string, string>
    {
        {"msyh", "msyh_lite"},
        {"msyhbd", "msyhbd_lite"},
        {"Hiragino Sans GB", "pglh_lite"},
        {"msyh_lite", "msyh_lite"},
        {"msyhbd_lite", "msyhbd_lite"},
        {"pglh_lite", "pglh_lite"},
    };

        static UIFontReplaceEditor()
        {
            EditorApplication.update += () =>
            {
                int replacedNumber = totalNumber - replaceQueue.Count;
                if (totalNumber != 0)
                {
                    string title = string.Format("替换字体中......{0}/{1}", replacedNumber, totalNumber);
                    string content = string.Format("正在替换字体请不要进行其他操作!!![{0}]", replaceQueue.Count > 0 ? replaceQueue.Peek() : "");
                    EditorUtility.DisplayProgressBar(title, content, (float)replacedNumber / (float)totalNumber);
                }

                if (replacedNumber >= totalNumber && replacedNumber != 0)
                {
                    EditorUtility.ClearProgressBar();
                    totalNumber = 0;
                    Debug.Log("字体替换完成!!!");
                    replacedNumber = 0;
                }

                if (replaceQueue.Count == 0)
                {
                    return;
                }

                if (replaceNowComplete == false)
                {
                    return;
                }

                replaceNowComplete = false;
                replaceFont(replaceQueue.Dequeue());
            };
        }

        static void replaceFont(string path)
        {
            string subPath = path.Replace(Directory.GetCurrentDirectory() + "\\", "");
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(subPath);
            bool hasText = false;
            foreach (var com in asset.GetComponentsInChildren<UnityEngine.Component>(true))
            {
                if (com is Text)
                {
                    hasText = true;
                }
            }

            if (!hasText)
            {
                replaceNowComplete = true;
                return;
            }

            if (asset == null)
            {
                replaceNowComplete = true;
                return;
            }
            GameObject instance = GameObject.Instantiate(asset);
            bool replaced = false;
            foreach (var tran in instance.GetComponentsInChildren<Transform>(true))
            {
                Text text = tran.GetComponent<Text>();
                if (text == null || text.font == null)
                {
                    continue;
                }

                string orFontName = text.font.name;
                if (!replaceMapping.ContainsKey(orFontName))
                {
                    continue;
                }

                string liteFontPath = string.Format("{0}/{1}.ttf", fontPath, replaceMapping[orFontName]);
                Font liteFont = AssetDatabase.LoadAssetAtPath<Font>(liteFontPath);

                if (liteFont == null)
                {
                    continue;
                }

                if (text.font == liteFont)
                {
                    continue;
                }

                text.font = liteFont;
                replaced = true;
            }

            if (replaced)
            {
                PrefabUtility.CreatePrefab(subPath.Replace("\\", "/"), instance);
            }
            DestroyImmediate(instance);
            AssetDatabase.Refresh();
            replaceNowComplete = true;
        }

        [MenuItem("Tools/UI/ReplaceFonts")]
        public static void ReplaceAllFonts()
        {
            Debug.Log("开始替换字体");
            totalNumber = 0;
            ReplaceFonts(rootPath);
        }

        static void ReplaceFonts(string dir)
        {
            foreach (string file in Directory.GetFiles(dir))
            {
                FileInfo fileInfo = new FileInfo(file);
                if (fileInfo.Extension != ".prefab")
                {
                    continue;
                }
                replaceQueue.Enqueue(fileInfo.FullName);
                totalNumber++;
            }

            if (Directory.GetDirectories(dir).Length > 0)
            {
                foreach (string subDir in Directory.GetDirectories(dir))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(subDir);
                    if (ignoreDir.Contains(directoryInfo.Name))
                    {
                        continue;
                    }
                    ReplaceFonts(subDir);
                }
            }
        }
    }
}