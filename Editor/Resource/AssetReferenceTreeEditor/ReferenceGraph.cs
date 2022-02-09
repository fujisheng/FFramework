using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Collections;
using UnityEngine;

namespace Framework.Service.Resource.Editor
{
    class ReferenceGraph
    {
        float xDistance = 50f;
        float yDistance = 50f;

        internal List<ReferenceNode> nodes => NodeExtensions.GetAllNodes();
        internal List<NodeConnection> connections;

        MapNode<IReference> root;

        internal ReferenceGraph(MapNode<IReference> root)
        {
            this.root = root;
            connections = new List<NodeConnection>();
        }

        internal void Clear()
        {
            nodes.Clear();
            connections.Clear();
        }

        internal void Draw()
        {
            connections.Clear();
            InitializeNodes(root, 0);
            CalculateInitialY(root);
            CheckAllChildrenOnScreen(root);
            CalculateFinalPositions(root, 0);

            nodes.ForEach(item => item.Draw());
            connections.ForEach(item => item.Draw());
        }

        internal void OnDrag(Vector2 delta)
        {
            nodes.ForEach(item => item.OnDrag(delta));
        }

        void InitializeNodes(MapNode<IReference> node, int depth)
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

        void CalculateFinalPositions(MapNode<IReference> node, float modSum)
        {
            var nodeRender = node.GetRender();
            nodeRender.OffsetY(modSum);
            modSum += nodeRender.Mod;

            foreach (var child in node.Children)
            {
                CalculateFinalPositions(child, modSum);
            }
        }

        void CalculateInitialY(MapNode<IReference> node)
        {
            var render = node.GetRender();

            foreach (var child in node.Children)
            {
                CalculateInitialY(child);
            }

            if (node.IsLeaf())
            {
                if (!node.IsUpMost())
                {
                    var preSibling = node.GetPreviousSibling().GetRender();
                    render.SetY(preSibling.Rect.y + preSibling.Rect.height + yDistance);
                }
                else
                {
                    render.SetY(0);
                }
            }
            else if (node.Children.Count == 1)
            {
                if (node.IsUpMost())
                {
                    render.SetY(node.Children[0].GetRender().Rect.y);
                }
                else
                {
                    var preSibling = node.GetPreviousSibling().GetRender();
                    render.SetY(preSibling.Rect.y + preSibling.Rect.height + yDistance);
                    render.SetMod(render.Rect.y - node.Children[0].GetRender().Rect.y);
                }
            }
            else
            {
                var leftChild = node.GetUpMostChild();
                var rightChild = node.GetDownMostChild();
                var mid = (leftChild.GetRender().Rect.y + rightChild.GetRender().Rect.y) / 2f;

                if (node.IsUpMost())
                {
                    render.SetY(mid);
                }
                else
                {
                    var preSibling = node.GetPreviousSibling().GetRender();
                    render.SetY(preSibling.Rect.y + preSibling.Rect.height + yDistance);
                    render.SetMod(render.Rect.y - mid);
                }
            }

            if (node.Children.Count > 0 && !node.IsUpMost())
            {
                CheckForConflicts(node);
            }
        }

        void CheckForConflicts(MapNode<IReference> node)
        {
            var render = node.GetRender();
            var minDistance = render.Rect.height + yDistance;
            var shiftValue = 0f;

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

        void CenterNodesBetween(MapNode<IReference> leftNode, MapNode<IReference> rightNode)
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

        void CheckAllChildrenOnScreen(MapNode<IReference> node)
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

        void GetUpContour(MapNode<IReference> node, float modSum, ref Dictionary<int, float> values)
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

        void GetDownContour(MapNode<IReference> node, float modSum, ref Dictionary<int, float> values)
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