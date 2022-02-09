using UnityEngine;

namespace Framework.Editor
{
    public class Node<T>
    {
        public T Value { get; private set; }
        public Rect Rect { get; private set; }

        Vector2 offset = Vector2.zero;

        public Rect RenderRect
        {
            get
            {
                return new Rect(Rect.x + offset.x, Rect.y + offset.y, Rect.width, Rect.height);
            }
        }

        public Node(Rect rect, T value)
        {
            this.Rect = rect;
            this.Value = value;
        }

        public void SetPosition(Vector2 position)
        {
            this.Rect = new Rect(position.x, position.y, Rect.width, Rect.height);
        }

        public void OffsetY(float yOffset)
        {
            this.Rect = new Rect(Rect.x, Rect.y + yOffset, Rect.width, Rect.height);
        }

        public void SetY(float y)
        {
            this.Rect = new Rect(Rect.x, y, Rect.width, Rect.height);
        }

        public virtual void OnDrag(Vector2 delta)
        {
            offset += delta;
        }

        public virtual void Draw()
        {
            GUI.Box(RenderRect, Value.ToString());
        }
    }
}