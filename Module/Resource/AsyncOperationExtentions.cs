using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Framework.Module.Resource
{
    public static class AsyncOperationExtentions
    {
        public static TaskAwaiter<T> GetAwaiter<T>(this AsyncOperationHandle<T> ap)
        {
            var tcs = new TaskCompletionSource<T>();
            ap.Completed += op => tcs.TrySetResult(op.Result);
            return tcs.Task.GetAwaiter();
        }
    }
}