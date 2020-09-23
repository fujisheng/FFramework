using System;

namespace Framework.Module.FSM
{
    public interface IFSM<T> where T : class
    {
        string Name { get; }
        T Owner { get; }
        int StateCount { get; }
        bool IsRunning { get; }
        bool IsDestroyed { get; }
        IState<T> CurrentState { get; }
        float CurrentStateTime { get; }
        void Start<TState>() where TState : IState<T>;
        void Start(Type stateType);
        bool HasState<TState>() where TState : IState<T>;
        bool HasState(Type stateType);
        TState GetState<TState>() where TState : IState<T>;
        IState<T> GetState(Type stateType);
        IState<T>[] GetAllStates();
        void Update();
        void Shutdown();
        void ChangeState<TState>() where TState : IState<T>;
        void ChangeState(Type stateType);
    }
}