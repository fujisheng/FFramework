using Cysharp.Threading.Tasks;

namespace Framework
{
    public interface IAsyncObjectPool<T>
    {
        int Size { get; }
        int Count { get; }
        float LastUseTime { get; }
        void SetSize(int size);
        void Push(T obj);
        UniTask<T> Pop();
        void Release();
    }
}