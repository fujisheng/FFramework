using System;
using System.Collections.Generic;

namespace Framework.Module.FSM
{
    internal sealed class FSMManager : Module , IFSMManager
    {
        readonly Dictionary<string, FSMBase> FSMS;
        readonly List<FSMBase> tempFSMS;

        public FSMManager()
        {
            FSMS = new Dictionary<string, FSMBase>();
            tempFSMS = new List<FSMBase>();
        }

        public int Count => FSMS.Count;

        internal override void OnUpdate()
        {
            tempFSMS.Clear();
            if (FSMS.Count <= 0)
            {
                return;
            }

            foreach (KeyValuePair<string, FSMBase> fsm in FSMS)
            {
                tempFSMS.Add(fsm.Value);
            }

            foreach (FSMBase fsm in tempFSMS)
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
            foreach (KeyValuePair<string, FSMBase> fsm in FSMS)
            {
                fsm.Value.Shutdown();
            }

            FSMS.Clear();
            tempFSMS.Clear();
        }

        public bool HasFSM<T>() where T : class
        {
            return InternalHasFSM(typeof(T).FullName);
        }

        public bool HasFSM(Type ownerType)
        {
            if (ownerType == null)
            {
                throw new Exception("Owner type is invalid.");
            }

            return InternalHasFSM(ownerType.FullName);
        }

        public bool HasFSM<T>(string name) where T : class
        {
            return InternalHasFSM(name);
        }

        public bool HasFsm(Type ownerType, string name)
        {
            if (ownerType == null)
            {
                throw new Exception("Owner type is invalid.");
            }

            return InternalHasFSM(ownerType.FullName);
        }

        public IFSM<T> GetFSM<T>() where T : class
        {
            return (IFSM<T>)InternelGetFSM(typeof(T).FullName);
        }

        public FSMBase GetFSM(Type ownerType)
        {
            if (ownerType == null)
            {
                throw new Exception("Owner type is invalid.");
            }

            return InternelGetFSM(ownerType.FullName);
        }

        public IFSM<T> GetFSM<T>(string name) where T : class
        {
            return (IFSM<T>)InternelGetFSM(name);
        }

        public FSMBase GetFSM(Type ownerType, string name)
        {
            if (ownerType == null)
            {
                throw new Exception("Owner type is invalid.");
            }

            return InternelGetFSM(ownerType.FullName);
        }

        public FSMBase[] GetAllFSM()
        {
            int index = 0;
            FSMBase[] fsms = new FSMBase[FSMS.Count];
            foreach (KeyValuePair<string, FSMBase> fsm in FSMS)
            {
                fsms[index++] = fsm.Value;
            }

            return fsms;
        }

        public IFSM<T> CreateFSM<T>(T owner, params IState<T>[] states) where T : class
        {
            return CreateFSM(string.Empty, owner, states);
        }

        public IFSM<T> CreateFSM<T>(string name, T owner, params IState<T>[] states) where T : class
        {
            if (HasFSM<T>(name))
            {
                throw new Exception(string.Format("Already exist FSM '{0}'.", name));
            }

            FSM<T> fsm = new FSM<T>(name, owner, states);
            FSMS.Add(name, fsm);
            return fsm;
        }

        public bool DestroyFSM<T>() where T : class
        {
            return InternalDestroyFSM(typeof(T).FullName);
        }

        public bool DestroyFSM(Type ownerType)
        {
            if (ownerType == null)
            {
                throw new Exception("Owner type is invalid.");
            }

            return InternalDestroyFSM(ownerType.FullName);
        }

        public bool DestroyFSM<T>(string name) where T : class
        {
            return InternalDestroyFSM(name);
        }

        public bool DestroyFSM(Type ownerType, string name)
        {
            if (ownerType == null)
            {
                throw new Exception("Owner type is invalid.");
            }

            return InternalDestroyFSM(ownerType.FullName);
        }

        public bool DestroyFSM<T>(IFSM<T> fsm) where T : class
        {
            if (fsm == null)
            {
                throw new Exception("FSM is invalid.");
            }

            return InternalDestroyFSM(typeof(T).FullName);
        }

        public bool DestroyFSM(FSMBase fsm)
        {
            if (fsm == null)
            {
                throw new Exception("FSM is invalid.");
            }

            return InternalDestroyFSM(fsm.OwnerType.FullName);
        }

        private bool InternalHasFSM(string fullName)
        {
            return FSMS.ContainsKey(fullName);
        }

        private FSMBase InternelGetFSM(string fullName)
        {
            if (FSMS.TryGetValue(fullName, out FSMBase fsm))
            {
                return fsm;
            }

            return null;
        }

        private bool InternalDestroyFSM(string fullName)
        {
            if (FSMS.TryGetValue(fullName, out FSMBase fsm))
            {
                fsm.Shutdown();
                return FSMS.Remove(fullName);
            }

            return false;
        }
    }
}