namespace Framework.Collections
{
    public class Map<T>
    {
        MapNode<T> root;

        private Map() { }

        /// <summary>
        /// 通过根节点值来创建一个图
        /// </summary>
        /// <param name="rootValue"></param>
        public Map(T rootValue)
        {
            this.root = new MapNode<T>(rootValue);
        }

        /// <summary>
        /// 根结点
        /// </summary>
        public MapNode<T> Root
        {
            get { return root; }
        }

        /// <summary>
        /// 查找某个值所对应的节点
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public MapNode<T> FindNode(T value)
        {
            return FindNode(root, value);
        }

        MapNode<T> FindNode(MapNode<T> previous, T value)
        {
            foreach (var child in previous.Children)
            {
                if (child.Value.Equals(value))
                {
                    return child;
                }
                var node = FindNode(child, value);
                if (node != null)
                {
                    return node;
                }
            }
            return null;
        }
    }
}