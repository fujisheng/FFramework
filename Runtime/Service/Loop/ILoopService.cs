using System;

namespace Framework.Service.Loop
{
    public interface ILoopService
    {
        void AddUpdate(Action update);
        void RemoveUpdate(Action update);
        void AddLateUpdate(Action lateUpdate);
        void RemoveLateUpdate(Action lateUpdate);
        void AddFixedUpdate(Action fixedUpdate);
        void RemoveFixedUpdate(Action fixedUpdate);
    }
}