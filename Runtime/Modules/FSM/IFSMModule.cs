using System;

namespace Framework.Module.FSM
{
    public interface IFSMModule
    {
        int Count { get; }
        bool Has<T>() where T : class;
        bool Has(Type ownerType);
        bool Has<T>(string name) where T : class;
        bool Has(Type ownerType, string name);
        IStateMachine<T> Get<T>() where T : class;
        StateMachineBase Get(Type ownerType);
        IStateMachine<T> Get<T>(string name) where T : class;
        StateMachineBase Get(Type ownerType, string name);
        StateMachineBase[] GetAll();
        IStateMachine<T> Create<T>(T owner, params IState<T>[] states) where T : class;
        IStateMachine<T> Create<T>(string name, T owner, params IState<T>[] states) where T : class;
        bool Destroy<T>() where T : class;
        bool Destroy(Type ownerType);
        bool Destroy<T>(string name) where T : class;
        bool Destroy(Type ownerType, string name);
        bool Destroy<T>(IStateMachine<T> fsm) where T : class;
        bool Destroy(StateMachineBase fsm);
    }
}
