using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Framework.Service.Threading
{
    /// <summary>
    /// 子线程回调到主线程管理
    /// </summary>
    internal sealed class ThreadService : Service, IThreadService
    {
        public static int maxThreads = 8;
        static int numThreads;
        List<Action> actions = new List<Action>();
        List<(float time, Action action)> delayed = new List<(float time, Action action)>();
        List<(float time, Action action)> currentDelayed = new List<(float time, Action action)>();
        List<Action> currentActions = new List<Action>();

        /// <summary>
        /// 回调到主线程
        /// </summary>
        /// <param name="action">回调</param>
        public void QueueOnMainThread(Action action)
        {
            QueueOnMainThread(action, 0f);
        }

        /// <summary>
        /// 回调到主线程
        /// </summary>
        /// <param name="action">回调</param>
        /// <param name="time">延迟时间</param>
        public void QueueOnMainThread(Action action, float time)
        {
            if (time != 0f)
            {
                lock (delayed)
                {
                    delayed.Add((Time.time + time, action));
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

        /// <summary>
        /// 开启一个子线程运行某个action
        /// </summary>
        /// <param name="a">任务</param>
        /// <returns></returns>
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