using System;

namespace Framework.Module.FSM
{
    public interface IFSMManager
    {
        int Count { get; }
        bool HasFSM<T>() where T : class;
        bool HasFSM(Type ownerType);
        bool HasFSM<T>(string name) where T : class;
        bool HasFsm(Type ownerType, string name);
        IFSM<T> GetFSM<T>() where T : class;
        FSMBase GetFSM(Type ownerType);
        IFSM<T> GetFSM<T>(string name) where T : class;
        FSMBase GetFSM(Type ownerType, string name);
        FSMBase[] GetAllFSM();
        IFSM<T> CreateFSM<T>(T owner, params IState<T>[] states) where T : class;
        IFSM<T> CreateFSM<T>(string name, T owner, params IState<T>[] states) where T : class;
        bool DestroyFSM<T>() where T : class;
        bool DestroyFSM(Type ownerType);
        bool DestroyFSM<T>(string name) where T : class;
        bool DestroyFSM(Type ownerType, string name);
        bool DestroyFSM<T>(IFSM<T> fsm) where T : class;
        bool DestroyFSM(FSMBase fsm);
    }
}
