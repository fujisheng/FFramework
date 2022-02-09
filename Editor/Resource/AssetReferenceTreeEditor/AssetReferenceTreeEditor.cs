using System.Collections.Generic;
using Framework.Collections;
using UnityEditor;
using UnityEngine;

namespace Framework.Service.Resource.Editor
{
    public class AssetReferenceTreeEditor : EditorWindow
    {
        const string AssetsReferenceTreeTypeName = "Framework.Service.Resource.AssetsReferenceTree";
        ReferenceGraph referenceGraph;

        [MenuItem("Window/AssetReferenceTree")]
        static void OpenWindow()
        {
            AssetReferenceTreeEditor window = GetWindow<AssetReferenceTreeEditor>();
            window.titleContent = new GUIContent("AssetReferenceTree");
        }

        private void OnEnable()
        {
            
        }

        private void OnGUI()
        {
            DrawMenu();
            referenceGraph?.Draw();
            ProcessEvents(Event.current);
        }

        void DrawMenu()
        {
            if (Application.isPlaying == false)
            {
                referenceGraph?.Clear();
                referenceGraph = null;
                GUI.Box(new Rect(0f, 0f, 200f, 20f), "必须在Play模式才能运行");
                return;
            }

            if (GUI.Button(new Rect(0, 0, 100, 20), "Clear"))
            {
                referenceGraph?.Clear();
                referenceGraph = null;
            }

            if (GUI.Button(new Rect(120, 0, 100, 20), new GUIContent("TakeSample")))
            {
                TakeSample();
            }
        }

        void TakeSample()
        {
            referenceGraph?.Clear();

            if(referenceGraph == null)
            {
                var flag = Utility.Assembly.AllFlag;
                var type = Utility.Assembly.GetType(AssetsReferenceTreeTypeName);
                if (type == null)
                {
                    UnityEngine.Debug.LogError("没有找到AssetsReferenceTree");
                    return;
                }

                var instance = type.GetProperty("Instance", flag).GetValue(null);
                UnityEngine.Debug.Log(type.BaseType.GetField("roots", flag).GetValue(instance));
                var roots = type.BaseType.GetField("roots", flag).GetValue(instance) as IEnumerable<MapNode<IReference>>;
                foreach (var @ref in roots)
                {
                    referenceGraph = new ReferenceGraph(@ref);
                }
            }
        }

        void ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDrag:
                    if (e.button == 2)
                    {
                        OnDrag(e.delta);
                    }
                    break;
            }
        }

        void OnDrag(Vector2 delta)
        {
            referenceGraph?.OnDrag(delta);
            GUI.changed = true;
        }
    }
}