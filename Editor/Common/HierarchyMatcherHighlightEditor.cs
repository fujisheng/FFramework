using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    [InitializeOnLoad]
    public class HierarchyMatcherHighlightEditor
    {
        static HierarchyMatcherHighlightEditor()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
        }

        static Color color = new Color(1, 11f / 255f, 242f / 255f, 1);
        private static void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            var obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (obj == null) { return; }

            if (obj.name[0] != '@')
            {
                return;
            }

            Rect rect = new Rect(selectionRect);
            rect.y += 1;
            rect.x += 18;
            GUIStyle style = new GUIStyle();
            style.normal.textColor = color;
            style.hover.textColor = color;
            GUI.Label(rect, "@", style);
        }
    }
}