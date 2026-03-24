using UnityEditor;
using UnityEngine;
using System.IO;

namespace Framework.Module.Resource.Editor
{
    /// <summary>
    /// ResourceSettings 编辑器工具
    /// 提供创建配置资源的菜单项
    /// </summary>
    public class ResourceSettingsEditor
    {
        const string SettingsPath = "Assets/Resources/ResourceSettings.asset";
        const string MenuPath = "Tools/Framework/Create Resource Settings";

        [MenuItem(MenuPath)]
        public static void CreateResourceSettings()
        {
            // 确保 Resources 文件夹存在
            if (!Directory.Exists("Assets/Resources"))
            {
                Directory.CreateDirectory("Assets/Resources");
                AssetDatabase.Refresh();
            }

            // 检查是否已存在
            var existing = AssetDatabase.LoadAssetAtPath<ResourceSettings>(SettingsPath);
            if (existing != null)
            {
                EditorUtility.DisplayDialog(
                    "Resource Settings Already Exists",
                    $"ResourceSettings already exists at:\n{SettingsPath}\n\nSelect it in Project window?",
                    "Select",
                    "Cancel"
                );
                
                Selection.activeObject = existing;
                EditorGUIUtility.PingObject(existing);
                return;
            }

            // 创建新的 ResourceSettings
            var settings = ScriptableObject.CreateInstance<ResourceSettings>();
            
            // 添加默认映射（从 AssetLoader 迁移的硬编码值）
            settings.RegisterMapping("Assets/Data/Prefabs/HomeView.prefab", "prefabs");
            settings.RegisterMapping("Assets/Data/Prefabs/TipsView.prefab", "prefabs");

            // 保存资源
            AssetDatabase.CreateAsset(settings, SettingsPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // 选中并高亮
            Selection.activeObject = settings;
            EditorGUIUtility.PingObject(settings);

            Debug.Log($"[ResourceSettingsEditor] Created ResourceSettings at: {SettingsPath}");
            Debug.Log("[ResourceSettingsEditor] Default mappings added. You can add more mappings in the Inspector.");
        }

        [MenuItem(MenuPath, true)]
        public static bool ValidateCreateResourceSettings()
        {
            // 始终启用菜单项
            return true;
        }

        /// <summary>
        /// 快速打开 ResourceSettings（用于代码中调用）
        /// </summary>        [MenuItem("Tools/Framework/Open Resource Settings")]
        public static void OpenResourceSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<ResourceSettings>(SettingsPath);
            if (settings != null)
            {
                Selection.activeObject = settings;
                EditorGUIUtility.PingObject(settings);
            }
            else
            {
                Debug.LogWarning($"[ResourceSettingsEditor] ResourceSettings not found at {SettingsPath}. Please create it first.");
                CreateResourceSettings();
            }
        }
    }
}
