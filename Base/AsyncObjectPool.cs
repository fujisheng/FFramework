using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public abstract class AsyncObjectPool<TObject> : IAsyncObjectPool<TObject> where TObject : class
    {
        protected Stack<TObject> pool = new Stack<TObject>();

        public virtual int Size { get; private set; }
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
        protected abstract UniTask<TObject> New();
        public async UniTask<TObject> Pop()
        {
            LastUseTime = Time.realtimeSinceStartup;

            if (pool.Count == 0)
            {
                return await New();
            }
            return pool.Pop();
        }

        public virtual void Release()
        {
            pool.Clear();
        }
    }
}