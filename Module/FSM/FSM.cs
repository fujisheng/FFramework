using System;
using System.Collections.Generic;

namespace Framework.Module.FSM
{
    public class FSM<T> : IFSM<T> where T : class
    {
        Dictionary<string, IState<T>> allStates;
        public string Name { get; private set; }
        public T Owner { get; private set; }
        public int StateCount { get; private set; }
        public bool IsRunning { get; private set; }
        public bool IsDestroyed { get; private set; }
        public IState<T> CurrentState { get; private set; }
        public float CurrentStateTime { get; private set; }


        public FSM(string name, T owner, params IState<T>[] states)
        {
            Name = name;
            Owner = owner;
            allStates = new Dictionary<string, IState<T>>();
            foreach(var state in states)
            {
                string stateName = state.GetType().FullName;
                if (allStates.ContainsKey(stateName))
                {
                    continue;
                }

                allStates.Add(stateName, state);
                state.OnInit(this);
            }
        }

        public void Start<TState>() where TState : IState<T>
        {
            if (IsRunning)
            {
                return;
            }

            IState<T> state = GetState<TState>();
            if(state == null)
            {
                return;
            }

            CurrentStateTime = 0f;
            CurrentState = state;
            CurrentState.OnEnter(this);
        }

        public void Start(Type stateType)
        {
            if (IsRunning)
            {
                return;
            }

            if(stateType == null)
            {
                return;
            }

            if (!typeof(IState<T>).IsAssignableFrom(stateType))
            {
                return;
            }

            IState<T> state = GetState(stateType);
            if(state == null)
            {
                return;
            }

            CurrentStateTime = 0f;
            CurrentState = state;
            CurrentState.OnEnter(this);
        }

        public bool HasState<TState>() where TState : IState<T>
        {
            return allStates.ContainsKey(typeof(TState).FullName);
        }

        public bool HasState(Type stateType)
        {
            if(stateType == null)
            {
                return false;
            }

            if (!typeof(IState<T>).IsAssignableFrom(stateType))
            {
                return false;
            }

            return allStates.ContainsKey(stateType.FullName);
        }

        public TState GetState<TState>() where TState : IState<T>
        {
            if (allStates.TryGetValue(typeof(TState).FullName, out IState<T> state))
            {
                return (TState)state;
            }

            return default;
        }

        public IState<T> GetState(Type stateType)
        {
            if(stateType == null)
            {
                return default;
            }

            if (!typeof(IState<T>).IsAssignableFrom(stateType))
            {
                return default;
            }

            if(allStates.TryGetValue(stateType.FullName, out IState<T> state))
            {
                return state;
            }

            return default;
        }

        public IState<T>[] GetAllStates()
        {
            int index = 0;
            IState<T>[] results = new IState<T>[allStates.Count];
            foreach(var kv in allStates)
            {
                results[index++] = kv.Value;
            }

            return results;
        }

        public void Update()
        {
            if(CurrentState == null)
            {
                return;
            }

            CurrentState.OnUpdate(this);
        }

        public void Shutdown()
        {
            if(CurrentState != null)
            {
                CurrentState.OnLeave(this, true);
                CurrentState = null;
                CurrentStateTime = 0f;
            }

            foreach(var kv in allStates)
            {
                kv.Value.OnDestroy(this);
            }

            allStates.Clear();
            IsDestroyed = true;
        }

        public void ChangeState<TState>() where TState : IState<T>
        {
            ChangeState(typeof(TState));
        }

        public void ChangeState(Type stateType)
        {
            if(CurrentState == null)
            {
                return;
            }

            IState<T> state = GetState(stateType);
            if(state == null)
            {
                return;
            }

            CurrentState.OnLeave(this, false);
            CurrentStateTime = 0f;
            CurrentState = state;
            CurrentState.OnEnter(this);
        }
    }
}

