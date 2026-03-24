using System.Collections.Generic;

namespace Framework.IoC
{
    /// <summary>
    /// Binding 对象池，减少 GC 压力
    /// </summary>
    internal static class BindingPool
    {
        static readonly Stack<Binding> pool = new Stack<Binding>();
        
        /// <summary>
        /// 当前池中对象数量
        /// </summary>
        internal static int Count => pool.Count;

        /// <summary>
        /// 将 Binding 放回池中
        /// </summary>
        internal static void Push(Binding binding)
        {
            binding.Dispose();
            pool.Push(binding);
        }

        /// <summary>
        /// 从池中取出一个 Binding
        /// </summary>
        internal static Binding Pop()
        {
            if (pool.Count == 0)
            {
                return new Binding();
            }
            return pool.Pop();
        }

        /// <summary>
        /// 清空池
        /// </summary>
        internal static void Clear()
        {
            pool.Clear();
        }
    }
}
