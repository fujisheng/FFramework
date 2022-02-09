using System.Collections.Generic;
using Framework.Collections;

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
        /// 存放等待被标记的引用
        /// </summary>
        Queue<ReferenceNode> markSet;

        public ReferenceTree()
        {
            roots = new List<ReferenceNode>();
            refSet = new List<ReferenceNode>();
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

            if (!refSet.Exists(item => item.Value.Equals(referenceNode.Value)))
            {
                refSet.Add(referenceNode);
            }
        }

        public void Alloc(IReference reference)
        {
            Alloc(null, reference);
        }

        public void Alloc(IReference root, IReference reference)
        {
            UnityEngine.Debug.Log($"alloc root:{root}  reference:{reference}");
            var rootNode = refSet.Find(item => item.Value.Equals(root));
            var referenceNode = refSet.Find(item => item.Value.Equals(reference));
            referenceNode = referenceNode ?? new ReferenceNode(reference);
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
                return;
            }

            var node = refSet.Find(item => item.Value.Equals(reference));
            if (node == null)
            {
                return;
            }

            if (!force && node.Children.Count != 0)
            {
                return;
            }
            node.Previous.ForEach(item => { item.RemoveChild(reference); Release(item.Value); });
        }

        /// <summary>
        /// 收集所有待释放的引用
        /// </summary>
        public void Collect()
        {
            refSet.ForEach(item => item.mark = Mark.White);
            roots.ForEach(item => { item.mark = Mark.Grey; markSet.Enqueue(item); });

            //标记
            while (true)
            {
                if (markSet.Count == 0)
                {
                    break;
                }

                var cur = markSet.Dequeue();
                cur.mark = Mark.Black;
                cur.Children.ForEach(child => { var node = (ReferenceNode)child; node.mark = Mark.Grey; markSet.Enqueue(node); });
            }
        }

        /// <summary>
        /// 删除需要释放的应用
        /// </summary>
        public void Delete()
        {
            //清除  这个地方倒序是因为可能后分配的会引用先分配的
            var whiteRefSet = refSet.FindAll(item => item.mark == Mark.White);
            whiteRefSet.Reverse();
            whiteRefSet.ForEach(item => item.Value.Release());
            refSet.RemoveAll(item => item.mark == Mark.White);
            roots.RemoveAll(item => item.mark == Mark.White);
            markSet.Clear();
        }
    }
}