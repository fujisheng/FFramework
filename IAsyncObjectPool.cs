using System.Threading.Tasks;

namespace Framework
{
    public interface IAsyncObjectPool<T>
    {
        int Size { get; }
        int Count { get; }
        float LastUseTime { get; }
        void SetSize(int size);
        void Push(T obj);
        Task<T> Pop();
        void Release();
    }
}