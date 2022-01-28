using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework.Service.Resource.Editor
{
    enum PortType { In, Out }

    class Port
    {
        public Rect rect;
        public PortType type;
        public ReferenceNode node;
        public GUIStyle style;

        public Port(ReferenceNode node, PortType type, GUIStyle style)
        {
            this.node = node;
            this.type = type;
            this.style = style;
            rect = new Rect(0, 0, 10f, 20f);
        }

        public void Draw()
        {
            rect.y = node.Rect.y + (node.Rect.height * 0.5f) - rect.height * 0.5f;
            rect.x = type == PortType.In ? node.Rect.x - rect.width + 8f : node.Rect.x + node.Rect.width - 8f;

            GUI.Box(rect, "", style);
        }
    }
}