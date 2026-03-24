using System;

namespace Framework.Module.FSM
{
    public class State<T> : IState<T> where T : class
    {
        public virtual void OnInitialize(IStateMachine<T> fsm) { }
        public virtual void OnEnter(IStateMachine<T> fsm) { }
        public virtual void OnUpdate(IStateMachine<T> fsm) { }
        public virtual void OnExit(IStateMachine<T> fsm, bool isShutdown) { }
        public virtual void OnDestroy(IStateMachine<T> fsm) { }
        public virtual void OnEvent(IStateMachine<T> fsm, object sender, int eventId, object args) { }

        public void ChangeState<TState>(IStateMachine<T> fsm) where TState : IState<T>
        {
            IStateMachine<T> fsmImplement = (IStateMachine<T>)fsm;
            if (fsmImplement == null)
            {
                return;
            }

            fsmImplement.ChangeState<TState>();
        }

        public void ChangeState(IStateMachine<T> fsm, Type stateType)
        {
            IStateMachine<T> fsmImplement = fsm;
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
