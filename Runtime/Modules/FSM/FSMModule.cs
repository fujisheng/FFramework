using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Module.FSM
{
    internal sealed class FSMModule : Module , IFSMModule
    {
        readonly Dictionary<string, StateMachineBase> stateMachines;
        readonly List<StateMachineBase> stateMachineTempList;

        public FSMModule()
        {
            stateMachines = new Dictionary<string, StateMachineBase>();
            stateMachineTempList = new List<StateMachineBase>();
        }

        public int Count => stateMachines.Count;

        internal override void OnUpdate()
        {
            stateMachineTempList.Clear();
            if (stateMachines.Count <= 0)
            {
                return;
            }

            foreach (KeyValuePair<string, StateMachineBase> fsm in stateMachines)
            {
                stateMachineTempList.Add(fsm.Value);
            }

            foreach (StateMachineBase fsm in stateMachineTempList)
            {
                if (fsm.IsDestroyed)
                {
                    continue;
                }

                fsm.Update();
            }
        }

        internal override void OnTearDown()
        {
            var values = stateMachines.Values.ToList();
            while (values.Count > 0)
            {
                var index = values.Count - 1;
                var fsm = values[index];
                fsm.Shutdown();
                values.RemoveAt(index);
            }

            stateMachines.Clear();
            stateMachineTempList.Clear();
        }

        public bool Has<T>() where T : class
        {
            return InternalHas(typeof(T).FullName);
        }

        public bool Has(Type ownerType)
        {
            Utility.Assert.IfNull(ownerType, new Exception("Owner type is invalid."));

            return InternalHas(ownerType.FullName);
        }

        public bool Has<T>(string name) where T : class
        {
            return InternalHas(name);
        }

        public bool Has(Type ownerType, string name)
        {
            Utility.Assert.IfNull(ownerType, new Exception("Owner type is invalid."));

            return InternalHas(ownerType.FullName);
        }

        public IStateMachine<T> Get<T>() where T : class
        {
            return (IStateMachine<T>)InternelGet(typeof(T).FullName);
        }

        public StateMachineBase Get(Type ownerType)
        {
            Utility.Assert.IfNull(ownerType, new Exception("Owner type is invalid."));

            return InternelGet(ownerType.FullName);
        }

        public IStateMachine<T> Get<T>(string name) where T : class
        {
            return (IStateMachine<T>)InternelGet(name);
        }

        public StateMachineBase Get(Type ownerType, string name)
        {
            Utility.Assert.IfNull(ownerType, new Exception("Owner type is invalid."));

            return InternelGet(ownerType.FullName);
        }

        public StateMachineBase[] GetAll()
        {
            int index = 0;
            StateMachineBase[] fsms = new StateMachineBase[stateMachines.Count];
            foreach (KeyValuePair<string, StateMachineBase> fsm in stateMachines)
            {
                fsms[index++] = fsm.Value;
            }

            return fsms;
        }

        public IStateMachine<T> Create<T>(T owner, params IState<T>[] states) where T : class
        {
            return Create(typeof(T).FullName, owner, states);
        }

        public IStateMachine<T> Create<T>(string name, T owner, params IState<T>[] states) where T : class
        {
            Utility.Assert.IfTrue(Has<T>(name), new Exception(string.Format("Already exist FSM '{0}'.", name)));

            StateMachine<T> fsm = new StateMachine<T>(name, owner, states);
            stateMachines.Add(name, fsm);
            return fsm;
        }

        public bool Destroy<T>() where T : class
        {
            return InternalDestroy(typeof(T).FullName);
        }

        public bool Destroy(Type ownerType)
        {
            Utility.Assert.IfNull(ownerType, new Exception("Owner type is invalid."));

            return InternalDestroy(ownerType.FullName);
        }

        public bool Destroy<T>(string name) where T : class
        {
            return InternalDestroy(name);
        }

        public bool Destroy(Type ownerType, string name)
        {
            Utility.Assert.IfNull(ownerType, new Exception("Owner type is invalid."));

            return InternalDestroy(ownerType.FullName);
        }

        public bool Destroy<T>(IStateMachine<T> fsm) where T : class
        {
            Utility.Assert.IfNull(fsm, new Exception("FSM is invalid."));

            return InternalDestroy(typeof(T).FullName);
        }

        public bool Destroy(StateMachineBase fsm)
        {
            Utility.Assert.IfNull(fsm, new Exception("FSM is invalid."));

            return InternalDestroy(fsm.OwnerType.FullName);
        }

        private bool InternalHas(string fullName)
        {
            return stateMachines.ContainsKey(fullName);
        }

        private StateMachineBase InternelGet(string fullName)
        {
            if (stateMachines.TryGetValue(fullName, out StateMachineBase fsm))
            {
                return fsm;
            }

            return null;
        }

        private bool InternalDestroy(string fullName)
        {
            if (stateMachines.TryGetValue(fullName, out StateMachineBase fsm))
            {
                fsm.Shutdown();
                return stateMachines.Remove(fullName);
            }

            return false;
        }
    }
}