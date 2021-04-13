using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public abstract class ObjectPool<TObject> : IObjectPool<TObject> where TObject : class
    {
        protected Stack<TObject> pool = new Stack<TObject>();
        int size = int.MaxValue;
        public virtual int Size { get { return size; } private set { size = value; } }
        public int Count { get { return pool.Count; } }
        public float LastUseTime { get; protected set; }

        public void SetSize(int size)
        {
            Size = size;
        }

        public void Push(TObject obj)
        {
            LastUseTime = Time.realtimeSinceStartup;
            if (pool.Count >= Size)
            {
                return;
            }

            pool.Push(obj);
        }

        public TObject Pop()
        {
            LastUseTime = Time.realtimeSinceStartup;

            if (pool.Count == 0)
            {
                return New();
            }
            return pool.Pop();
        }

        public virtual void Dispose()
        {
            pool.Clear();
        }

        protected abstract TObject New();
    }
}

