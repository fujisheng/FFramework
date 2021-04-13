using System;

namespace Framework.Module.Loop
{
    public interface ILoopManager
    {
        void AddUpdate(Action update);
        void RemoveUpdate(Action update);
        void AddLateUpdate(Action lateUpdate);
        void RemoveLateUpdate(Action lateUpdate);
        void AddFixedUpdate(Action fixedUpdate);
        void RemoveFixedUpdate(Action fixedUpdate);
    }
}