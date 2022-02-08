using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Framework.Collections;
using Framework.Editor;
using UnityEditor;
using UnityEngine;

namespace Framework.Service.Resource.Editor
{
    public class AssetReferenceTreeEditor : EditorWindow
    {
        const string AssetsReferenceTreeTypeName = "Framework.Service.Resource.AssetsReferenceTree";
        Dropdown runModel;

        [MenuItem("Window/AssetReferenceTree")]
        static void OpenWindow()
        {
            AssetReferenceTreeEditor window = GetWindow<AssetReferenceTreeEditor>();
            window.titleContent = new GUIContent("AssetReferenceTree");
        }

        public enum RunType
        {
            Runtime = 2,
            Click = 3,
        };

        private void OnEnable()
        {
            runModel = new Dropdown(new Rect(110, 0, 100, 30), typeof(RunType));
        }

        private void OnGUI()
        {
            DrawMenu();
            Utility.Nodes?.ForEach(item => item.Draw());
            Utility.connections?.ForEach(item => item.Draw());

            ProcessEvents(Event.current);
        }

        void DrawMenu()
        {
            if (Application.isPlaying == false)
            {
                Utility.Clear();
                GUI.Box(new Rect(0f, 0f, 200f, 30f), "必须在Play模式才能运行");
                return;
            }

            if (GUI.Button(new Rect(0, 0, 100, 30), "Clear"))
            {
                Utility.Clear();
            }

            runModel.Draw();
            if (runModel.SelectedValue == (int)RunType.Click)
            {
                if (GUI.Button(new Rect(220, 0, 100, 30), new GUIContent("TakeSample")))
                {
                    TakeSample();
                }
            }
            else
            {
                TakeSample();
            }
        }

        void TakeSample()
        {
            Utility.Clear();

            var flag = Framework.Utility.Assembly.AllFlag;
            var type = Framework.Utility.Assembly.GetType(AssetsReferenceTreeTypeName);
            if (type == null)
            {
                UnityEngine.Debug.LogError("没有找到AssetsReferenceTree");
                return;
            }
            var property = type.GetProperty("Instance", flag);
            var instance = property.GetValue(null);
            var roots = instance.GetType().GetMethod("GetRoots").Invoke(instance, null) as IEnumerable<MapNode<IReference>>;

            foreach (var @ref in roots)
            {
                Utility.CalculateNodePositions(@ref);
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
            Utility.Nodes?.ForEach(item => item.OnDrag(delta));
            GUI.changed = true;
        }
    }
}