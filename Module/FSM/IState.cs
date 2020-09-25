using System;

namespace Framework.Module.FSM
{
    public interface IState<T> where T : class
    {
        void OnInit(IFSM<T> fsm);
        void OnEnter(IFSM<T> fsm);
        void OnUpdate(IFSM<T> fsm);
        void OnLeave(IFSM<T> fsm, bool isShutdown);
        void OnDestroy(IFSM<T> fsm);
        void ChangeState<TState>(IFSM<T> fsm) where TState : IState<T>;
        void ChangeState(IFSM<T> fsm, Type stateType);
        void OnEvent(IFSM<T> fsm, object sender, int eventId, object args);
    }
}