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

        Vector2 offset = Vector2.zero;

        public Rect RenderRect
        {
            get
            {
                return new Rect(rect.x + offset.x, rect.y + offset.y, rect.width, rect.height);
            }
        }

        public Port(ReferenceNode node, PortType type, GUIStyle style)
        {
            this.node = node;
            this.type = type;
            this.style = style;
            rect = new Rect(0, 0, 10f, 20f);
        }

        public void OnDrag(Vector2 delta)
        {
            offset += delta;
        }

        public void Draw()
        {
            rect.y = node.RenderRect.y + (node.RenderRect.height * 0.5f) - rect.height * 0.5f;
            rect.x = type == PortType.In ? node.RenderRect.x - rect.width + 8f : node.RenderRect.x + node.RenderRect.width - 8f;

            GUI.Box(rect, "", style);
        }
    }
}