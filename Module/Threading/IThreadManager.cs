using System;
using System.Threading;

namespace Framework.Module.Threading
{
    public interface IThreadManager
    {
        void QueueOnMainThread(Action action);
        void QueueOnMainThread(Action action, float delayTime);
        Thread RunAsync(Action action);
    }
}