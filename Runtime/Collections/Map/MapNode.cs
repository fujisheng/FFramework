using System.Collections.Generic;

namespace Framework.Collections
{
    public class MapNode<T>
    {
        readonly List<MapNode<T>> previous;
        readonly T value;
        readonly List<MapNode<T>> children;

        private MapNode() { }

        /// <summary>
        /// 通过一个值来创建一个节点
        /// </summary>
        /// <param name="value"></param>
        public MapNode(T value)
        {
            this.value = value;
            this.previous = new List<MapNode<T>>(1);
            this.children = new List<MapNode<T>>(2);
        }

        /// <summary>
        /// 值
        /// </summary>
        public T Value
        {
            get { return value; }
        }

        /// <summary>
        /// 父节点
        /// </summary>
        public List<MapNode<T>> Previous
        {
            get { return previous; }
        }

        /// <summary>
        /// 子节点
        /// </summary>
        public List<MapNode<T>> Children
        {
            get { return children; }
        }

        /// <summary>
        /// 添加一个子节点
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(MapNode<T> child)
        {
            if (Children.Exists(item => item.Value.Equals(child.value)))
            {
                return;
            }
            Children.Add(child);
            child.AddPrevious(this);
        }

        /// <summary>
        /// 根据子节点的值移除子节点
        /// </summary>
        /// <param name="childValue"></param>
        public void RemoveChild(T childValue)
        {
            // 找到要移除的子节点
            var child = Children.Find(item => item.Value.Equals(childValue));
            if (child != null)
            {
                // 从子节点的 Previous 中移除当前节点（维护双向边）
                child.Previous.Remove(this);
                // 从当前节点的 Children 中移除子节点
                Children.Remove(child);
            }
        }

        public void AddPrevious(MapNode<T> previous)
        {
            if (Previous.Exists(item => item.value.Equals(previous.value)))
            {
                return;
            }
            Previous.Add(previous);
        }
    }
}