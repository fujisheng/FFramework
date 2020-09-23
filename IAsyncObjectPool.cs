using System.Threading.Tasks;

namespace Framework
{
    public interface IAsyncObjectPool<T> : IObjectPool<T>
    {
        new Task<T> Pop();
        new Task<T> New();
    }
}