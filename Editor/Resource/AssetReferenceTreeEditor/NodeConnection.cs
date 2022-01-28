using UnityEngine;

namespace Framework.Service.Resource.Editor
{
    class NodeConnection
    {
        public Port inPort;
        public Port outPort;

        public NodeConnection(Port inPort, Port outPort)
        {
            this.inPort = inPort;
            this.outPort = outPort;
        }

        public void Draw()
        {
            Framework.Editor.Utility.Line.DrawArrow(inPort.rect.center, outPort.rect.center, Color.white, Framework.Editor.Utility.Line.ArrowType.Mid);
        }
    }
}