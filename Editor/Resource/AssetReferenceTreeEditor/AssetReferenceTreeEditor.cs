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
        List<ReferenceNode> nodes;
        List<NodeConnection> connections;

        GUIStyle nodeStyle;
        GUIStyle activeStyle;
        GUIStyle inPortStyle;
        GUIStyle outPortStyle;

        Dropdown runModel;

        Vector2 startPos = new Vector2(100, 100);

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
            nodes = new List<ReferenceNode>();
            connections = new List<NodeConnection>();

            runModel = new Dropdown(new Rect(110, 0, 100, 30), typeof(RunType));

            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
            nodeStyle.border = new RectOffset(12, 12, 12, 12);

            activeStyle = new GUIStyle();
            activeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
            activeStyle.border = new RectOffset(12, 12, 12, 12);

            inPortStyle = new GUIStyle();
            inPortStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
            inPortStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
            inPortStyle.border = new RectOffset(4, 4, 12, 12);

            outPortStyle = new GUIStyle();
            outPortStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
            outPortStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
            outPortStyle.border = new RectOffset(4, 4, 12, 12);
        }

        private void OnGUI()
        {
            DrawMenu();

            nodes?.ForEach(item => item.Draw());
            connections?.ForEach(item => item.Draw());

            ProcessEvents(Event.current);
        }

        void DrawMenu()
        {
            if (Application.isPlaying == false)
            {
                nodes.Clear();
                connections.Clear();
                GUI.Box(new Rect(0f, 0f, 200f, 30f), "必须在Play模式才能运行");
                return;
            }

            if (GUI.Button(new Rect(0, 0, 100, 30), "Clear"))
            {
                nodes.Clear();
                connections.Clear();
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
            nodes.Clear();
            connections.Clear();

            var flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;
            Type type = null;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GetType("AssetsReferenceTree") != null)
                {
                    type = assembly.GetType("AssetsReferenceTree");
                }
            }
            if (type == null)
            {
                UnityEngine.Debug.LogError("没有找到AssetsReferenceTree");
                return;
            }
            var property = type.GetProperty("Instance", flag);
            var instance = property.GetValue(null);
            var roots = instance.GetType().GetMethod("GetRoots").Invoke(instance, null) as IEnumerable<MapNode<IReference>>;

            var index = 0;
            foreach (var @ref in roots)
            {
                var nodePos = new Vector2(startPos.x, startPos.y + 100f * index);
                BuildTree(@ref, nodePos);
                index++;
            }
        }

        void BuildTree(MapNode<IReference> node, Vector2 startPos)
        {
            var nodeRect = new Rect(startPos.x, startPos.y, 150, 50);
            var refNode = GetOrCreateNode(nodeRect, node);
            var childIndex = 0;
            var childStartX = startPos.x + 200f;
            var childStartY = startPos.y;
            foreach (var child in node.Children)
            {
                var childPos = new Vector2(childStartX, childStartY + 100f * childIndex);
                var childRect = new Rect(childPos.x, childPos.y, 150, 50);
                var childNode = GetOrCreateNode(childRect, child);
                NodeConnection connection = new NodeConnection(refNode.outPort, childNode.inPort);
                connections.Add(connection);
                childIndex++;
                BuildTree(child, childPos);
            }
        }

        ReferenceNode GetOrCreateNode(Rect rect, MapNode<IReference> value)
        {
            var node = nodes.FirstOrDefault(item => item.Value == value.Value);
            if (node != null)
            {
                return node;
            }
            node = new ReferenceNode(rect, value.Value, nodeStyle, nodeStyle, activeStyle, inPortStyle, outPortStyle);
            nodes.Add(node);
            return node;
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
            startPos += delta;
            nodes?.ForEach(item => item.OnDrag(delta));
            GUI.changed = true;
        }
    }
}