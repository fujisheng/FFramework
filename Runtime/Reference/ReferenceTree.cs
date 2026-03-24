using System;
using System.Collections.Generic;
using Framework.Collections;
using Framework.Module.Resource;

namespace Framework
{
    public class ReferenceTree
    {
        /// <summary>
        /// 所有的根节点
        /// </summary>
        List<ReferenceNode> roots;

        /// <summary>
        /// 存放所有的引用
        /// </summary>
        List<ReferenceNode> refSet;

        /// <summary>
        /// 快速查找引用的字典 O(1)查找
        /// </summary>
        Dictionary<IReference, ReferenceNode> refMap;

        /// <summary>
        /// 存放等待被标记的引用
        /// </summary>
        Queue<ReferenceNode> markSet;

        public ReferenceTree()
        {
            roots = new List<ReferenceNode>();
            refSet = new List<ReferenceNode>();
            refMap = new Dictionary<IReference, ReferenceNode>();
            markSet = new Queue<ReferenceNode>();
        }

        void AllocInternal(ReferenceNode root, ReferenceNode referenceNode)
        {
            if (root == null && !roots.Exists(item => item.Value.Equals(referenceNode.Value)))
            {
                roots.Add(referenceNode);
            }
            else
            {
                root.AddChild(referenceNode);
            }

            if (!refMap.ContainsKey(referenceNode.Value))
            {
                refSet.Add(referenceNode);
                refMap[referenceNode.Value] = referenceNode;
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// 检测添加引用关系是否会形成循环
        /// 使用DFS从child开始遍历其祖先节点，如果能到达parent则会形成循环
        /// </summary>
        /// <param name="parent">父节点</param>
        /// <param name="child">子节点</param>
        /// <returns>如果会形成循环返回true，否则返回false</returns>
        bool WouldCreateCycle(ReferenceNode parent, ReferenceNode child)
        {
            if (parent == null || child == null)
            {
                return false;
            }

            // 检查child的祖先链是否能到达parent
            var visited = new HashSet<ReferenceNode>();
            var stack = new Stack<ReferenceNode>();
            stack.Push(child);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                
                // 找到了parent，说明会形成循环
                if (current == parent)
                {
                    return true;
                }

                // 避免无限循环
                if (!visited.Add(current))
                {
                    continue;
                }

                // 遍历所有父节点
                foreach (var prev in current.Previous)
                {
                    stack.Push((ReferenceNode)prev);
                }
            }

            return false;
        }
#endif

        public void Alloc(IReference reference)
        {
            Alloc(null, reference);
        }

        public void Alloc(IReference root, IReference reference)
        {
            ResourceLogger.Verbose("ReferenceTree", $"Alloc root:{root} reference:{reference}");
            refMap.TryGetValue(root, out var rootNode);
            if (!refMap.TryGetValue(reference, out var referenceNode))
            {
                referenceNode = new ReferenceNode(reference);
            }
            
#if UNITY_EDITOR
            // 检测是否会形成循环引用
            if (rootNode != null && WouldCreateCycle(rootNode, referenceNode))
            {
                ResourceLogger.Warning("ReferenceTree", $"Cycle detected: {reference} -> {root}. Skipping allocation.");
                return;
            }
#endif
            
            AllocInternal(rootNode, referenceNode);
        }

        /// <summary>
        /// 释放一个应用
        /// </summary>
        /// <param name="reference">引用</param>
        /// <param name="force">强制释放  如果为true 则不管这个引用还有没有被其它引用着都会被卸载掉 反之还被引用着就不会卸载</param>
        public void Release(IReference reference, bool force = false)
        {
            if (reference == null)
            {
                ResourceLogger.Verbose("ReferenceTree", "Release failed: reference is null");
                return;
            }

            if (!refMap.TryGetValue(reference, out var node))
            {
                ResourceLogger.Verbose("ReferenceTree", $"Release failed: reference not found - {reference}");
                return;
            }

            if (!force && node.Children.Count != 0)
            {
                ResourceLogger.Verbose("ReferenceTree", $"Release skipped: {reference} has {node.Children.Count} children (force={force})");
                return;
            }
            ResourceLogger.Verbose("ReferenceTree", $"Release: {reference} (force={force})");
            node.Previous.ForEach(item => { item.RemoveChild(reference); Release(item.Value); });
        }

        /// <summary>
        /// 收集所有待释放的引用
        /// </summary>
        public void Collect()
        {
            ResourceLogger.Verbose("ReferenceTree", $"Collect: roots={roots.Count}, refs={refSet.Count}");
            refSet.ForEach(item => item.mark = Mark.White);
            roots.ForEach(item => { item.mark = Mark.Grey; markSet.Enqueue(item); });

            //标记
            while (markSet.Count > 0)
            {
                var cur = markSet.Dequeue();
                
                // 只有White节点才需要处理（防止循环引用导致的重复处理）
                if (cur.mark != Mark.White)
                {
                    continue;
                }
                
                cur.mark = Mark.Black;
                
                // 只将White子节点标记为Grey并入队
                foreach (var child in cur.Children)
                {
                    var node = (ReferenceNode)child;
                    if (node.mark == Mark.White)
                    {
                        node.mark = Mark.Grey;
                        markSet.Enqueue(node);
                    }
                }
            }

#if DEBUG_RESOURCE
            var whiteCount = refSet.FindAll(item => item.mark == Mark.White).Count;
            ResourceLogger.Verbose("ReferenceTree", $"Collect completed: {whiteCount} references marked for deletion");
#endif
        }

        /// <summary>
        /// 删除需要释放的应用
        /// </summary>
        public void Delete()
        {
            //清除  这个地方倒序是因为可能后分配的会引用先分配的
            var whiteRefSet = refSet.FindAll(item => item.mark == Mark.White);
            ResourceLogger.Verbose("ReferenceTree", $"Delete: {whiteRefSet.Count} references to release");
            whiteRefSet.Reverse();
            foreach (var item in whiteRefSet)
            {
                ResourceLogger.Verbose("ReferenceTree", $"  Deleting: {item.Value}");
            }
            // 清理被删除节点的边连接
            foreach (var whiteNode in whiteRefSet)
            {
                // 从父节点的 Children 中移除
                foreach (var parent in whiteNode.Previous)
                {
                    parent.Children.Remove(whiteNode);
                }
                whiteNode.Previous.Clear();
                
                // 从子节点的 Previous 中移除
                foreach (var child in whiteNode.Children)
                {
                    child.Previous.Remove(whiteNode);
                }
                whiteNode.Children.Clear();
                
                // 释放资源
                whiteNode.Value.Release();
            }
            
            refSet.RemoveAll(item => item.mark == Mark.White);
            roots.RemoveAll(item => item.mark == Mark.White);
            
            // 从 refMap 中移除已释放的引用
            foreach (var whiteNode in whiteRefSet)
            {
                refMap.Remove(whiteNode.Value);
            }
            
            markSet.Clear();
            ResourceLogger.Verbose("ReferenceTree", $"Delete completed: {refSet.Count} refs, {roots.Count} roots remaining");
        }
    }
}