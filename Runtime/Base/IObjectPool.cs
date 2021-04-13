using System;

namespace Framework
{
    public interface IObjectPool<T> : IDisposable
    {
        int Size { get; }
        int Count { get; }
        float LastUseTime { get; }
        void SetSize(int size);
        void Push(T obj);
        T Pop();
    }
}