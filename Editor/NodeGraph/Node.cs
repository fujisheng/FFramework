using UnityEngine;

namespace Framework.Editor
{
    public class Node<T>
    {
        public T Value { get; private set; }
        public Rect Rect { get; private set; }

        public Node(Rect rect, T value)
        {
            this.Rect = rect;
            this.Value = value;
        }

        public virtual void OnDrag(Vector2 delta)
        {
            var position = Rect.position + delta;
            this.Rect = new Rect(position.x, position.y, Rect.width, Rect.height);
        }

        public virtual void Draw()
        {
            GUI.Box(Rect, Value.ToString());
        }
    }
}