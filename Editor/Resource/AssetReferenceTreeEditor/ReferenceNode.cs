using Framework.Editor;
using UnityEditor;
using UnityEngine;

namespace Framework.Service.Resource.Editor
{
    class ReferenceNode : Node<IReference>
    {
        public Port inPort;
        public Port outPort;

        public GUIStyle style;
        public GUIStyle defaultStyle;
        public GUIStyle activeStyle;

        public float Mod { get; private set; }

        public ReferenceNode(Rect rect, IReference value) : base(rect, value)
        {
            style = new GUIStyle();
            style.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
            style.border = new RectOffset(12, 12, 12, 12);

            activeStyle = new GUIStyle();
            activeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
            activeStyle.border = new RectOffset(12, 12, 12, 12);

            var inPortStyle = new GUIStyle();
            inPortStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
            inPortStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
            inPortStyle.border = new RectOffset(4, 4, 12, 12);

            var outPortStyle = new GUIStyle();
            outPortStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
            outPortStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
            outPortStyle.border = new RectOffset(4, 4, 12, 12);

            inPort = new Port(this, PortType.In, inPortStyle);
            outPort = new Port(this, PortType.Out, outPortStyle);
        }

        public ReferenceNode(IReference value) : this(new Rect(0f, 0f, 150f, 50f), value) { }

        public void SetMod(float mod)
        {
            this.Mod = mod;
        }

        public override void Draw()
        {
            inPort.Draw();
            outPort.Draw();
            GUI.Box(RenderRect, "", style);
            GUI.Label(new Rect(RenderRect.x + 10, RenderRect.y + 5, RenderRect.width - 20f, RenderRect.height), Value?.ToString());

            var size = Framework.Editor.Utility.Memory.GetUnityObjectFieldPropertyRuntimeMemorySize(Value);
            GUI.Label(new Rect(RenderRect.x + 10, RenderRect.y + 15, RenderRect.width - 20f, RenderRect.height), $"{size / 1000f}kb");
        }
    }
}