using System.Collections.Generic;
using System.Linq;
using Framework.Collections;

namespace Framework.Service.Resource.Editor
{
    static class NodeExtensions
    {
        static Dictionary<IReference, ReferenceNode> mapping = new Dictionary<IReference, ReferenceNode>();

        internal static bool IsLeaf(this MapNode<IReference> value)
        {
            return value.Children.Count == 0;
        }

        internal static bool IsUpMost(this MapNode<IReference> value)
        {
            if (value.Previous.Count == 0)
            {
                return true;
            }
            return value.Previous[0].Children[0] == value;
        }

        internal static bool IsDownMost(this MapNode<IReference> value)
        {
            if (value.Previous.Count == 0)
            {
                return true;
            }

            return value.Previous[0].Children[value.Previous[0].Children.Count - 1] == value;
        }

        internal static MapNode<IReference> GetPreviousSibling(this MapNode<IReference> value)
        {
            if (value.Previous.Count == 0 || value.IsUpMost())
            {
                return null;
            }

            return value.Previous[0].Children[value.Previous[0].Children.IndexOf(value) - 1];
        }

        internal static MapNode<IReference> GetNextSibling(this MapNode<IReference> value)
        {
            if (value.Previous.Count == 0 || value.IsDownMost())
            {
                return null;
            }

            return value.Previous[0].Children[value.Previous[0].Children.IndexOf(value) + 1];
        }

        internal static MapNode<IReference> GetUpMostSibling(this MapNode<IReference> value)
        {
            if (value.Previous.Count == 0)
            {
                return null;
            }

            if (value.IsUpMost())
            {
                return value;
            }

            return value.Previous[0].Children[0];
        }

        internal static MapNode<IReference> GetUpMostChild(this MapNode<IReference> value)
        {
            if (value.Children.Count == 0)
            {
                return null;
            }
            return value.Children[0];
        }

        internal static MapNode<IReference> GetDownMostChild(this MapNode<IReference> value)
        {
            if (value.Children.Count == 0)
            {
                return null;
            }
            return value.Children[value.Children.Count - 1];
        }

        internal static ReferenceNode GetRender(this MapNode<IReference> value)
        {
            if (mapping.TryGetValue(value.Value, out var node))
            {
                return node;
            }

            node = new ReferenceNode(value.Value);
            mapping.Add(value.Value, node);
            return node;
        }

        internal static List<ReferenceNode> GetAllNodes()
        {
            return mapping.Values.ToList();
        }

        internal static void ClearNodes()
        {
            mapping.Clear();
        }
    }
}