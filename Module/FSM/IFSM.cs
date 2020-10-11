using System;

namespace Framework.Module.FSM
{
    public interface IFSM<T> where T : class
    {
        string Name{ get;}
        T Owner{ get; }
        int StateCount { get; }
        bool IsRunning{get;}
        bool IsDestroyed{get;}
        IState<T> CurrentState{ get;}
        float CurrentStateTime{ get;}
        void Start<TState>() where TState : IState<T>;
        void Start(Type stateType);
        void ChangeState<TState>() where TState : IState<T>;
        void ChangeState(Type stateType);
        bool HasState<TState>() where TState : IState<T>;
        bool HasState(Type stateType);
        TState GetState<TState>() where TState : IState<T>;
        IState<T> GetState(Type stateType);
        void FireEvent(object sender, int eventId);
        void FireEvent(object sender, int eventId, object userData);
        bool HasData(string name);
        TData GetData<TData>(string name) where TData : IArgs;
        IArgs GetData(string name);
        void SetData<TData>(string name, TData data) where TData : IArgs;
        void SetData(string name, IArgs data);
        bool RemoveData(string name);
    }
}