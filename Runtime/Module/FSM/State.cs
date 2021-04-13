using System;

namespace Framework.Module.FSM
{
    public class State<T> : IState<T> where T : class
    {
        public virtual void OnInit(IFSM<T> fsm) { }
        public virtual void OnEnter(IFSM<T> fsm) { }
        public virtual void OnUpdate(IFSM<T> fsm) { }
        public virtual void OnLeave(IFSM<T> fsm, bool isShutdown) { }
        public virtual void OnDestroy(IFSM<T> fsm) { }
        public virtual void OnEvent(IFSM<T> fsm, object sender, int eventId, object args) { }

        public void ChangeState<TState>(IFSM<T> fsm) where TState : IState<T>
        {
            IFSM<T> fsmImplement = (IFSM<T>)fsm;
            if (fsmImplement == null)
            {
                return;
            }

            fsmImplement.ChangeState<TState>();
        }

        public void ChangeState(IFSM<T> fsm, Type stateType)
        {
            IFSM<T> fsmImplement = fsm;
            if (fsmImplement == null)
            {
                return;
            }

            if (stateType == null)
            {
                return;
            }

            if (!typeof(IState<T>).IsAssignableFrom(stateType))
            {
                return;
            }

            fsmImplement.ChangeState(stateType);
        }
    }
}
