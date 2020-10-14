using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Framework.Module.Threading
{
    struct DelayedQueueItem
    {
        public float time;
        public Action action;
    }

    internal sealed class ThreadManager : Module, IThreadManager
    {
        public static int maxThreads = 8;
        static int numThreads;
        List<Action> actions = new List<Action>();
        List<DelayedQueueItem> delayed = new List<DelayedQueueItem>();
        List<DelayedQueueItem> currentDelayed = new List<DelayedQueueItem>();
        List<Action> currentActions = new List<Action>();

        public void QueueOnMainThread(Action action)
        {
            QueueOnMainThread(action, 0f);
        }

        public void QueueOnMainThread(Action action, float time)
        {
            if (time != 0f)
            {
                lock (delayed)
                {
                    delayed.Add(new DelayedQueueItem { time = Time.time + time, action = action });
                }
            }
            else
            {
                lock (actions)
                {
                    actions.Add(action);
                }
            }
        }

        public Thread RunAsync(Action a)
        {
            while (numThreads >= maxThreads)
            {
                Thread.Sleep(1);
            }
            Interlocked.Increment(ref numThreads);
            ThreadPool.QueueUserWorkItem(RunAction, a);
            return null;
        }

        void RunAction(object action)
        {
            try
            {
                ((Action)action)();
            }
            catch
            {
            }
            finally
            {
                Interlocked.Decrement(ref numThreads);
            }
        }

        internal override void OnUpdate()
        {
            lock (actions)
            {
                currentActions.Clear();
                currentActions.AddRange(actions);
                actions.Clear();
            }
            foreach (var a in currentActions)
            {
                a.Invoke();
            }
            lock (delayed)
            {
                currentDelayed.Clear();
                currentDelayed.AddRange(delayed.Where(d => d.time <= Time.time));
                foreach (var item in currentDelayed)
                {
                    delayed.Remove(item);
                }
            }
            foreach (var delayed in currentDelayed)
            {
                delayed.action();
            }
        }
    }
}