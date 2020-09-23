using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Framework.Module.Message
{
    public interface IMessage<T>
    {
        void AddListener(string message, Action<T> listener);
        void RemoveListener(string message, Action<T> listener);
        void SendMessage(string message, T args);
    }

    public class MessageBase<T> : IMessage<T>
    {

        Dictionary<string, Action<T>> listeners = new Dictionary<string, Action<T>>();

        public void AddListener(string message, Action<T> listener)
        {
            if (listeners.ContainsKey(message))
            {
                listeners[message] += listener;
                return;
            }

            listeners.Add(message, listener);
        }

        public void RemoveListener(string message, Action<T> listener)
        {
            if (!listeners.ContainsKey(message))
            {
                return;
            }

            if (listeners[message] == null)
            {
                return;
            }

            listeners[message] -= listener;
        }

        public void SendMessage(string message, T args = default(T))
        {
            if (!listeners.ContainsKey(message))
            {
                return;
            }

            if (listeners[message] == null)
            {
                return;
            }

            listeners[message].Invoke(args);
        }
    }
}

