using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;

namespace Framework.Async
{
    public struct AsyncFVoidMethodBuilder
    {
        private Action moveNext;

        [DebuggerHidden]
        public static AsyncFVoidMethodBuilder Create()
        {
            AsyncFVoidMethodBuilder builder = new AsyncFVoidMethodBuilder();
            return builder;
        }

        public FVoid Task => default;

        [DebuggerHidden]
        public void SetException(Exception exception)
        {
            UnityEngine.Debug.LogError(exception);
        }

        [DebuggerHidden]
        public void SetResult()
        {
            // do nothing
        }

        [DebuggerHidden]
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            if (moveNext == null)
            {
                var runner = new MoveNextRunner<TStateMachine>();
                moveNext = runner.Run;
                runner.StateMachine = stateMachine;
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