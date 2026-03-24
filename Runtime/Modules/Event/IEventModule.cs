using System;

namespace Framework.Module.Event
{
    public interface IEventModule
    {
        IEventChannel<T> GetChannel<T>();
        void RemoveChannel<T>();
        void RemoveChannel(Type type);
        void Release();
    }
}