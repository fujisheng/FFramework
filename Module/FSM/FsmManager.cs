﻿using System;
using System.Collections.Generic;

namespace Framework.Module.FSM
{
    internal sealed class FSMManager : Module 
    {
        private readonly Dictionary<string, FSMBase> m_Fsms;
        private readonly List<FSMBase> m_TempFsms;

        public FSMManager()
        {
            m_Fsms = new Dictionary<string, FSMBase>();
            m_TempFsms = new List<FSMBase>();
        }

        public int Count { get { return m_Fsms.Count; } }

        internal override void OnUpdate()
        {
            m_TempFsms.Clear();
            if (m_Fsms.Count <= 0)
            {
                return;
            }

            foreach (KeyValuePair<string, FSMBase> fsm in m_Fsms)
            {
                m_TempFsms.Add(fsm.Value);
            }

            foreach (FSMBase fsm in m_TempFsms)
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
            foreach (KeyValuePair<string, FSMBase> fsm in m_Fsms)
            {
                fsm.Value.Shutdown();
            }

            m_Fsms.Clear();
            m_TempFsms.Clear();
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
            FSMBase[] fsms = new FSMBase[m_Fsms.Count];
            foreach (KeyValuePair<string, FSMBase> fsm in m_Fsms)
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
            m_Fsms.Add(name, fsm);
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
            return m_Fsms.ContainsKey(fullName);
        }

        private FSMBase InternelGetFSM(string fullName)
        {
            FSMBase fsm = null;
            if (m_Fsms.TryGetValue(fullName, out fsm))
            {
                return fsm;
            }

            return null;
        }

        private bool InternalDestroyFSM(string fullName)
        {
            FSMBase fsm = null;
            if (m_Fsms.TryGetValue(fullName, out fsm))
            {
                fsm.Shutdown();
                return m_Fsms.Remove(fullName);
            }

            return false;
        }
    }
}