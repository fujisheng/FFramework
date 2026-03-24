using System;

namespace Framework.Module.FSM
{
    public interface IState<T> where T : class
    {
        void OnInitialize(IStateMachine<T> fsm);
        void OnEnter(IStateMachine<T> fsm);
        void OnUpdate(IStateMachine<T> fsm);
        void OnExit(IStateMachine<T> fsm, bool isShutdown);
        void OnDestroy(IStateMachine<T> fsm);
        void ChangeState<TState>(IStateMachine<T> fsm) where TState : IState<T>;
        void ChangeState(IStateMachine<T> fsm, Type stateType);
        void OnEvent(IStateMachine<T> fsm, object sender, int eventId, object args);
    }
}