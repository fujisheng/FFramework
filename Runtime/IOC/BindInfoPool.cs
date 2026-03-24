using System.Collections.Generic;

namespace FInject
{
    internal static class BindInfoPool
    {
        static Stack<BindInfo> pool = new Stack<BindInfo>();
        internal static int Count { get { return pool.Count; } }

        internal static void Push(BindInfo obj)
        {
            obj.Dispose();
            pool.Push(obj);
        }

        internal static BindInfo Pop()
        {
            if (pool.Count == 0)
            {
                return new BindInfo();
            }
            return pool.Pop();
        }

        internal static void Dispose()
        {
            pool.Clear();
        }
    }
}

