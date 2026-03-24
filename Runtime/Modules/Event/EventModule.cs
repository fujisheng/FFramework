using System;
using System.Collections.Generic;

namespace Framework.Module.Event
{
    internal class EventModule : Module, IEventModule
    {
        readonly Dictionary<Type, object> channels;

        public EventModule()
        {
            channels = new Dictionary<Type, object>();
        }

        public IEventChannel<T> GetChannel<T>()
        {
            var type = typeof(T);
            if (!channels.TryGetValue(type, out var channel))
            {
                channel = new EventChannel<T>();
                channels[type] = channel;
            }
            return (IEventChannel<T>)channel;
        }

        public void RemoveChannel<T>()
        {
            RemoveChannel(typeof(T));
        }

        public void RemoveChannel(Type type)
        {
            channels.Remove(type);
        }

        public void Release()
        {
            foreach (var channel in channels.Values)
            {
                (channel as IDisposable).Dispose();
            }
            channels.Clear();
        }
    }
}
