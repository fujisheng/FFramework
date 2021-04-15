using System;
using System.Threading;

namespace Framework.Service.Threading
{
    public interface IThreadService
    {
        void QueueOnMainThread(Action action);
        void QueueOnMainThread(Action action, float delayTime);
        Thread RunAsync(Action action);
    }
}