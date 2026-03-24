using System;

namespace Framework.Module.Loop
{
    public interface ILoopModule
    {
        void AddUpdate(Action<float> update);
        void RemoveUpdate(Action<float> update);
        void AddLateUpdate(Action<float> lateUpdate);
        void RemoveLateUpdate(Action<float> lateUpdate);
        void AddFixedUpdate(Action<float> fixedUpdate);
        void RemoveFixedUpdate(Action<float> fixedUpdate);
    }
}