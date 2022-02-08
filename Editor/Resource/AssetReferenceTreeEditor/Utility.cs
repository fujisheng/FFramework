using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Collections;
using UnityEngine;

namespace Framework.Service.Resource.Editor
{
    static class Extension
    {
        internal static Dictionary<IReference, ReferenceNode> mapping = new Dictionary<IReference, ReferenceNode>();

        public static bool IsLeaf(this MapNode<IReference> value)
        {
            return value.Children.Count == 0;
        }

        public static bool IsUpMost(this MapNode<IReference> value)
        {
            if(value.Previous.Count == 0)
            {
                return true;
            }
            return value.Previous[0].Children[0] == value;
        }

        public static bool IsDownMost(this MapNode<IReference> value)
        {
            if(value.Previous.Count == 0)
            {
                return true;
            }

            return value.Previous[0].Children[value.Previous[0].Children.Count - 1] == value;
        }

        public static MapNode<IReference> GetPreviousSibling(this MapNode<IReference> value)
        {
            if (value.Previous.Count == 0 || value.IsUpMost())
            {
                return null;
            }

            return value.Previous[0].Children[value.Previous[0].Children.IndexOf(value) - 1];
        }

        public static MapNode<IReference> GetNextSibling(this MapNode<IReference> value)
        {
            if (value.Previous.Count == 0 || value.IsDownMost())
            {
                return null;
            }

            return value.Previous[0].Children[value.Previous[0].Children.IndexOf(value) + 1];
        }

        public static MapNode<IReference> GetUpMostSibling(this MapNode<IReference> value)
        {
            if(value.Previous.Count == 0)
            {
                return null;
            }

            if (value.IsUpMost())
            {
                return value;
            }

            return value.Previous[0].Children[0];
        }

        public static MapNode<IReference> GetUpMostChild(this MapNode<IReference> value)
        {
            if(value.Children.Count == 0)
            {
                return null;
            }
            return value.Children[0];
        }

        public static MapNode<IReference> GetDownMostChild(this MapNode<IReference> value)
        {
            if(value.Children.Count == 0)
            {
                return null;
            }
            return value.Children[value.Children.Count - 1];
        }

        public static ReferenceNode GetRender(this MapNode<IReference> value)
        {
            if (mapping.TryGetValue(value.Value, out var node))
            {
                return node;
            }

            node = new ReferenceNode(value.Value);
            mapping.Add(value.Value, node);
            return node;
        }
    }
    static class Utility
    {
        static float xDistance = 50f;
        static int nodeSize = 100;
        static float siblingDistance = 100f;
        static float treeDistance = 100f;

        internal static List<NodeConnection> connections = new List<NodeConnection>();

        internal static List<ReferenceNode> Nodes
        {
            get { return Extension.mapping.Values.ToList(); }
        }

        internal static void Clear()
        {
            Extension.mapping.Clear();
            connections.Clear();
        }

        internal static void CalculateNodePositions(MapNode<IReference> root)
        {
            InitializeNodes(root, 0);
            //CalculateInitialX(root);
            //CheckAllChildrenOnScreen(root);
            //CalculateFinalPositions(root, 0);
        }

        static void InitializeNodes(MapNode<IReference> node, int depth)
        {
            var nodeRender = node.GetRender();
            nodeRender.SetPosition(new Vector2(depth * (nodeRender.Rect.width + xDistance), -1f));
            nodeRender.SetMod(0);

            foreach (var child in node.Children)
            {
                connections.Add(new NodeConnection(nodeRender.outPort, child.GetRender().inPort));
                InitializeNodes(child, depth + 1);
            }   
        }

        static void CalculateFinalPositions(MapNode<IReference> node, float modSum)
        {
            var nodeRender = node.GetRender();
            nodeRender.OffsetY(modSum);
            modSum += nodeRender.Mod;

            foreach (var child in node.Children)
            {
                CalculateFinalPositions(child, modSum);
            } 
        }

        static void CalculateInitialY(MapNode<IReference> node)
        {
            var render = node.GetRender();

            foreach (var child in node.Children)
            {
                CalculateInitialY(child);
            }
                
            if (node.IsLeaf())
            {
                if (!node.IsUpMost())
                    render.SetY(node.GetPreviousSibling().GetRender().Rect.y + nodeSize + siblingDistance);
                else
                    render.SetY(0);
            }
            else if (node.Children.Count == 1)
            {
                if (node.IsUpMost())
                {
                    render.SetY(node.Children[0].GetRender().Rect.y);
                }
                else
                {
                    render.SetY(node.GetPreviousSibling().GetRender().Rect.y + nodeSize + siblingDistance);
                    render.SetMod(render.Rect.y - node.Children[0].GetRender().Rect.y);
                }
            }
            else
            {
                var leftChild = node.GetUpMostChild();
                var rightChild = node.GetDownMostChild();
                var mid = (leftChild.GetRender().Rect.y + rightChild.GetRender().Rect.y) / 2;

                if (node.IsUpMost())
                {
                    render.SetY(mid);
                }
                else
                {
                    render.SetY(node.GetPreviousSibling().GetRender().Rect.y + nodeSize + siblingDistance);
                    render.SetMod(render.Rect.y - mid);
                }
            }

            if (node.Children.Count > 0 && !node.IsUpMost())
            {
                CheckForConflicts(node);
            }
        }

        static void CheckForConflicts(MapNode<IReference> node)
        {
            var render = node.GetRender();
            var minDistance = treeDistance + nodeSize;
            var shiftValue = 0F;

            var nodeContour = new Dictionary<int, float>();
            GetUpContour(node, 0, ref nodeContour);

            var sibling = node.GetUpMostSibling();
            while (sibling != null && sibling != node)
            {
                var siblingContour = new Dictionary<int, float>();
                GetDownContour(sibling, 0, ref siblingContour);

                for (int level = (int)render.Rect.y + 1; level <= Math.Min(siblingContour.Keys.Max(), nodeContour.Keys.Max()); level++)
                {
                    var distance = nodeContour[level] - siblingContour[level];
                    if (distance + shiftValue < minDistance)
                    {
                        shiftValue = minDistance - distance;
                    }
                }

                if (shiftValue > 0)
                {
                    render.OffsetY(shiftValue);
                    render.SetMod(render.Mod + shiftValue);

                    CenterNodesBetween(node, sibling);

                    shiftValue = 0;
                }

                sibling = sibling.GetNextSibling();
            }
        }

        static void CenterNodesBetween(MapNode<IReference> leftNode, MapNode<IReference> rightNode)
        {
            var leftIndex = leftNode.Previous[0].Children.IndexOf(rightNode);
            var rightIndex = leftNode.Previous[0].Children.IndexOf(leftNode);

            var numNodesBetween = (rightIndex - leftIndex) - 1;

            if (numNodesBetween > 0)
            {
                var distanceBetweenNodes = (leftNode.GetRender().Rect.x - rightNode.GetRender().Rect.x) / (numNodesBetween + 1);

                int count = 1;
                for (int i = leftIndex + 1; i < rightIndex; i++)
                {
                    var middleNode = leftNode.Previous[0].Children[i];

                    var desiredX = rightNode.GetRender().Rect.x + (distanceBetweenNodes * count);
                    var offset = desiredX - middleNode.GetRender().Rect.x;
                    middleNode.GetRender().OffsetY(offset);
                    middleNode.GetRender().SetMod(middleNode.GetRender().Mod + offset);

                    count++;
                }

                CheckForConflicts(leftNode);
            }
        }

        static void CheckAllChildrenOnScreen(MapNode<IReference> node)
        {
            var nodeContour = new Dictionary<int, float>();
            GetUpContour(node, 0, ref nodeContour);

            float shiftAmount = 0;
            foreach (var y in nodeContour.Keys)
            {
                if (nodeContour[y] + shiftAmount < 0)
                    shiftAmount = (nodeContour[y] * -1);
            }

            if (shiftAmount > 0)
            {
                node.GetRender().OffsetY(shiftAmount);
                node.GetRender().SetMod(node.GetRender().Mod + shiftAmount);
            }
        }

        static void GetUpContour(MapNode<IReference> node, float modSum, ref Dictionary<int, float> values)
        {
            var render = node.GetRender();
            if (!values.ContainsKey((int)render.Rect.y))
                values.Add((int)render.Rect.y, render.Rect.x + modSum);
            else
                values[(int)render.Rect.y] = Math.Min(values[(int)render.Rect.y], render.Rect.x + modSum);

            modSum += render.Mod;
            foreach (var child in node.Children)
            {
                GetUpContour(child, modSum, ref values);
            }
        }

        static void GetDownContour(MapNode<IReference> node, float modSum, ref Dictionary<int, float> values)
        {
            var render = node.GetRender();
            if (!values.ContainsKey((int)render.Rect.y))
                values.Add((int)render.Rect.y, render.Rect.x + modSum);
            else
                values[(int)render.Rect.y] = Math.Max(values[(int)render.Rect.y], render.Rect.x + modSum);

            modSum += render.Mod;
            foreach (var child in node.Children)
            {
                GetDownContour(child, modSum, ref values);
            }
        }
    }
}
