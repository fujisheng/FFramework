using System;
using System.Threading;

namespace Framework.Async
{
    public partial struct FTask
    {
        public static FTask CompletedTask => new FTask();

        public static FTask FromException(Exception ex)
        {
            FTaskCompletionSource tcs = new FTaskCompletionSource();
            tcs.TrySetException(ex);
            return tcs.Task;
        }

        public static FTask<T> FromException<T>(Exception ex)
        {
            var tcs = new FTaskCompletionSource<T>();
            tcs.TrySetException(ex);
            return tcs.Task;
        }

        public static FTask<T> FromResult<T>(T value)
        {
            return new FTask<T>(value);
        }

        public static FTask FromCanceled()
        {
            return CanceledFTaskCache.Task;
        }

        public static FTask<T> FromCanceled<T>()
        {
            return CanceledFTaskCache<T>.Task;
        }

        public static FTask FromCanceled(CancellationToken token)
        {
            FTaskCompletionSource tcs = new FTaskCompletionSource();
            tcs.TrySetException(new OperationCanceledException(token));
            return tcs.Task;
        }

        public static FTask<T> FromCanceled<T>(CancellationToken token)
        {
            var tcs = new FTaskCompletionSource<T>();
            tcs.TrySetException(new OperationCanceledException(token));
            return tcs.Task;
        }
        
        private static class CanceledFTaskCache
        {
            public static readonly FTask Task;

            static CanceledFTaskCache()
            {
                FTaskCompletionSource tcs = new FTaskCompletionSource();
                tcs.TrySetCanceled();
                Task = tcs.Task;
            }
        }

        private static class CanceledFTaskCache<T>
        {
            public static readonly FTask<T> Task;

            static CanceledFTaskCache()
            {
                var taskCompletionSource = new FTaskCompletionSource<T>();
                taskCompletionSource.TrySetCanceled();
                Task = taskCompletionSource.Task;
            }
        }
    }

    internal static class CompletedTasks
    {
        public static readonly FTask<bool> True = FTask.FromResult(true);
        public static readonly FTask<bool> False = FTask.FromResult(false);
        public static readonly FTask<int> Zero = FTask.FromResult(0);
        public static readonly FTask<int> MinusOne = FTask.FromResult(-1);
        public static readonly FTask<int> One = FTask.FromResult(1);
    }
}