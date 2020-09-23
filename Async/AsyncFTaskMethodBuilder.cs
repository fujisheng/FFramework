using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;

namespace Framework.Async
{
    public struct AsyncFTaskMethodBuilder
    {
        private FTaskCompletionSource tcs;
        private Action moveNext;

        [DebuggerHidden]
        public static AsyncFTaskMethodBuilder Create()
        {
            AsyncFTaskMethodBuilder builder = new AsyncFTaskMethodBuilder();
            return builder;
        }

        [DebuggerHidden]
        public FTask Task
        {
            get
            {
                if (this.tcs != null)
                {
                    return this.tcs.Task;
                }

                if (moveNext == null)
                {
                    return FTask.CompletedTask;
                }

                this.tcs = new FTaskCompletionSource();
                return this.tcs.Task;
            }
        }

        [DebuggerHidden]
        public void SetException(Exception exception)
        {
            if (this.tcs == null)
            {
                this.tcs = new FTaskCompletionSource();
            }

            if (exception is OperationCanceledException ex)
            {
                this.tcs.TrySetCanceled(ex);
            }
            else
            {
                this.tcs.TrySetException(exception);
            }
        }

        [DebuggerHidden]
        public void SetResult()
        {
            if (moveNext == null)
            {
            }
            else
            {
                if (this.tcs == null)
                {
                    this.tcs = new FTaskCompletionSource();
                }

                this.tcs.TrySetResult();
            }
        }

        [DebuggerHidden]
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
                where TAwaiter : INotifyCompletion
                where TStateMachine : IAsyncStateMachine
        {
            if (moveNext == null)
            {
                if (this.tcs == null)
                {
                    this.tcs = new FTaskCompletionSource(); // built future.
                }

                var runner = new MoveNextRunner<TStateMachine>();
                moveNext = runner.Run;
                runner.StateMachine = stateMachine; // set after create delegate.
            }

            awaiter.OnCompleted(moveNext);
        }

        [DebuggerHidden]
        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
                where TAwaiter : ICriticalNotifyCompletion
                where TStateMachine : IAsyncStateMachine
        {
            if (moveNext == null)
            {
                if (this.tcs == null)
                {
                    this.tcs = new FTaskCompletionSource(); // built future.
                }

                var runner = new MoveNextRunner<TStateMachine>();
                moveNext = runner.Run;
                runner.StateMachine = stateMachine; // set after create delegate.
            }

            awaiter.UnsafeOnCompleted(moveNext);
        }

        [DebuggerHidden]
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            stateMachine.MoveNext();
        }

        [DebuggerHidden]
        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
        }
    }

    public struct ETAsyncTaskMethodBuilder<T>
    {
        private T result;
        private FTaskCompletionSource<T> tcs;
        private Action moveNext;

        [DebuggerHidden]
        public static ETAsyncTaskMethodBuilder<T> Create()
        {
            var builder = new ETAsyncTaskMethodBuilder<T>();
            return builder;
        }

        [DebuggerHidden]
        public FTask<T> Task
        {
            get
            {
                if (this.tcs != null)
                {
                    return new FTask<T>(this.tcs);
                }

                if (moveNext == null)
                {
                    return new FTask<T>(result);
                }

                this.tcs = new FTaskCompletionSource<T>();
                return this.tcs.Task;
            }
        }

        [DebuggerHidden]
        public void SetException(Exception exception)
        {
            if (this.tcs == null)
            {
                this.tcs = new FTaskCompletionSource<T>();
            }

            if (exception is OperationCanceledException ex)
            {
                this.tcs.TrySetCanceled(ex);
            }
            else
            {
                this.tcs.TrySetException(exception);
            }
        }

        [DebuggerHidden]
        public void SetResult(T ret)
        {
            if (moveNext == null)
            {
                this.result = ret;
            }
            else
            {
                if (this.tcs == null)
                {
                    this.tcs = new FTaskCompletionSource<T>();
                }

                this.tcs.TrySetResult(ret);
            }
        }

        [DebuggerHidden]
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
                where TAwaiter : INotifyCompletion
                where TStateMachine : IAsyncStateMachine
        {
            if (moveNext == null)
            {
                if (this.tcs == null)
                {
                    this.tcs = new FTaskCompletionSource<T>(); // built future.
                }

                var runner = new MoveNextRunner<TStateMachine>();
                moveNext = runner.Run;
                runner.StateMachine = stateMachine; // set after create delegate.
            }

            awaiter.OnCompleted(moveNext);
        }

        [DebuggerHidden]
        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
                where TAwaiter : ICriticalNotifyCompletion
                where TStateMachine : IAsyncStateMachine
        {
            if (moveNext == null)
            {
                if (this.tcs == null)
                {
                    this.tcs = new FTaskCompletionSource<T>();
                }

                var runner = new MoveNextRunner<TStateMachine>();
                moveNext = runner.Run;
                runner.StateMachine = stateMachine;
            }

            awaiter.UnsafeOnCompleted(moveNext);
        }

        [DebuggerHidden]
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            stateMachine.MoveNext();
        }

        [DebuggerHidden]
        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
        }
    }
}