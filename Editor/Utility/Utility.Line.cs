using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    public static partial class Utility
    {
        public static class Line
        {
            public enum ArrowType
            {
                Mid,
                End,
            }
            public static void DrawArrow(Vector2 from, Vector2 to, Color color, ArrowType arrowType = ArrowType.End)
            {
                Handles.BeginGUI();
                Handles.color = color;
                Handles.DrawAAPolyLine(3, from, to);
                var v0 = from - to;
                v0 *= 10 / v0.magnitude;
                var v1 = new Vector2(v0.x * 0.866f - v0.y * 0.5f, v0.x * 0.5f + v0.y * 0.866f);
                var v2 = new Vector2(v0.x * 0.866f + v0.y * 0.5f, v0.x * -0.5f + v0.y * 0.866f);

                var fromTo = to - from;
                var p = arrowType == ArrowType.End ? to : GetMidPosition(from, to);
                Handles.DrawAAPolyLine(3, p + v1, p, p + v2);
                Handles.EndGUI();
            }

            public static Vector2 GetMidPosition(Vector2 from, Vector2 to)
            {
                var fromTo = to - from;
                var mid = from + fromTo / 2f;
                return mid;
            }
        }
    }
}