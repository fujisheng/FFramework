using Framework.Editor;
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

        public ReferenceNode(Rect rect, IReference value, GUIStyle nodeStyle, GUIStyle defaultStyle, GUIStyle activeStyle, GUIStyle inPortStyle, GUIStyle outPortStyle) : base(rect, value)
        {
            style = nodeStyle;
            inPort = new Port(this, PortType.In, inPortStyle);
            outPort = new Port(this, PortType.Out, outPortStyle);
            this.defaultStyle = defaultStyle;
            this.activeStyle = activeStyle;
        }

        public override void Draw()
        {
            inPort.Draw();
            outPort.Draw();
            GUI.Box(Rect, "", style);
            GUI.Label(new Rect(Rect.x + 10, Rect.y + 5, Rect.width - 20f, Rect.height), Value?.ToString());

            var size = Framework.Editor.Utility.Memory.GetUnityObjectFieldPropertyRuntimeMemorySize(Value);
            GUI.Label(new Rect(Rect.x + 10, Rect.y + 15, Rect.width - 20f, Rect.height), $"{size / 1000f}kb");
        }
    }
}