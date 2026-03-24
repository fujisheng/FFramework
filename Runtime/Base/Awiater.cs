using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using UnityEngine;

namespace Framework
{
    public static class AsyncOperationExtensions
    {
        public static TaskAwaiter GetAwaiter(this AsyncOperation asyncOperation)
        {
            var tcs = new TaskCompletionSource<object>();
            asyncOperation.completed += _ => tcs.SetResult(null);
            return ((Task)tcs.Task).GetAwaiter();
        }
    }
}
