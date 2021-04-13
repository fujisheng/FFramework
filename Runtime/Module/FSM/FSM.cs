using System;
using System.Collections.Generic;

namespace Framework.Module.FSM
{
    internal sealed class FSM<T> : FSMBase, IFSM<T> where T : class
    {
        readonly Dictionary<string, IState<T>> states;
        readonly Dictionary<string, IArgs> datas;
        float currentStateTime;
        bool isDestroyed;

        /// <summary>
        /// 获取有限状态机持有者。
        /// </summary>
        public T Owner { get; }

        /// <summary>
        /// 获取当前有限状态机状态。
        /// </summary>
        public IState<T> CurrentState { get; private set; }

        /// <summary>
        /// 获取有限状态机持有者类型。
        /// </summary>
        public override Type OwnerType => typeof(T);

        /// <summary>
        /// 获取有限状态机中状态的数量。
        /// </summary>
        public override int StateCount => states.Count;

        /// <summary>
        /// 获取有限状态机是否正在运行。
        /// </summary>
        public override bool IsRunning => CurrentState != null;

        /// <summary>
        /// 获取有限状态机是否被销毁。
        /// </summary>
        public override bool IsDestroyed => isDestroyed;

        /// <summary>
        /// 获取当前有限状态机状态名称。
        /// </summary>
        public override string CurrentStateName => CurrentState?.GetType().FullName;

        /// <summary>
        /// 获取当前有限状态机状态持续时间。
        /// </summary>
        public override float CurrentStateTime => currentStateTime;

        /// <summary>
        /// 初始化有限状态机的新实例。
        /// </summary>
        /// <param name="name">有限状态机名称。</param>
        /// <param name="owner">有限状态机持有者。</param>
        /// <param name="states">有限状态机状态集合。</param>
        public FSM(string name, T owner, params IState<T>[] states) : base(name)
        {
            if (owner == null)
            {
                throw new Exception("FSM owner is invalid.");
            }

            if (states == null || states.Length < 1)
            {
                throw new Exception("FSM states is invalid.");
            }

            this.Owner = owner;
            this.states = new Dictionary<string, IState<T>>();
            datas = new Dictionary<string, IArgs>();

            foreach (IState<T> state in states)
            {
                if (state == null)
                {
                    throw new Exception("FSM states is invalid.");
                }

                string stateName = state.GetType().FullName;
                if (this.states.ContainsKey(stateName))
                {
                    throw new Exception(string.Format("FSM '{0}' state '{1}' is already exist.", name, stateName));
                }

                this.states.Add(stateName, state);
                state.OnInit(this);
            }

            currentStateTime = 0f;
            CurrentState = null;
            isDestroyed = false;
        }

        /// <summary>
        /// 开始有限状态机。
        /// </summary>
        /// <typeparam name="TState">要开始的有限状态机状态类型。</typeparam>
        public void Start<TState>() where TState : IState<T>
        {
            if (IsRunning)
            {
                throw new Exception("FSM is running, can not start again.");
            }

            IState<T> state = GetState<TState>();
            if (state == null)
            {
                throw new Exception(string.Format("FSM '{0}' can not start state '{1}' which is not exist.", Name, typeof(TState).FullName));
            }

            currentStateTime = 0f;
            CurrentState = state;
            CurrentState.OnEnter(this);
        }

        /// <summary>
        /// 开始有限状态机。
        /// </summary>
        /// <param name="stateType">要开始的有限状态机状态类型。</param>
        public void Start(Type stateType)
        {
            if (IsRunning)
            {
                throw new Exception("FSM is running, can not start again.");
            }

            if (stateType == null)
            {
                throw new Exception("State type is invalid.");
            }

            if (!typeof(IState<T>).IsAssignableFrom(stateType))
            {
                throw new Exception(string.Format("State type '{0}' is invalid.", stateType.FullName));
            }

            IState<T> state = GetState(stateType);
            if (state == null)
            {
                throw new Exception(string.Format("FSM '{0}' can not start state '{1}' which is not exist.", Name, stateType.FullName));
            }

            currentStateTime = 0f;
            CurrentState = state;
            CurrentState.OnEnter(this);
        }

        /// <summary>
        /// 是否存在有限状态机状态。
        /// </summary>
        /// <typeparam name="TState">要检查的有限状态机状态类型。</typeparam>
        /// <returns>是否存在有限状态机状态。</returns>
        public bool HasState<TState>() where TState : IState<T>
        {
            return states.ContainsKey(typeof(TState).FullName);
        }

        /// <summary>
        /// 是否存在有限状态机状态。
        /// </summary>
        /// <param name="stateType">要检查的有限状态机状态类型。</param>
        /// <returns>是否存在有限状态机状态。</returns>
        public bool HasState(Type stateType)
        {
            if (stateType == null)
            {
                throw new Exception("State type is invalid.");
            }

            if (!typeof(IState<T>).IsAssignableFrom(stateType))
            {
                throw new Exception(string.Format("State type '{0}' is invalid.", stateType.FullName));
            }

            return states.ContainsKey(stateType.FullName);
        }

        /// <summary>
        /// 获取有限状态机状态。
        /// </summary>
        /// <typeparam name="TState">要获取的有限状态机状态类型。</typeparam>
        /// <returns>要获取的有限状态机状态。</returns>
        public TState GetState<TState>() where TState : IState<T>
        {
            if (states.TryGetValue(typeof(TState).FullName, out IState<T> state))
            {
                return (TState)state;
            }

            return default;
        }

        /// <summary>
        /// 获取有限状态机状态。
        /// </summary>
        /// <param name="stateType">要获取的有限状态机状态类型。</param>
        /// <returns>要获取的有限状态机状态。</returns>
        public IState<T> GetState(Type stateType)
        {
            if (stateType == null)
            {
                throw new Exception("State type is invalid.");
            }

            if (!typeof(IState<T>).IsAssignableFrom(stateType))
            {
                throw new Exception(string.Format("State type '{0}' is invalid.", stateType.FullName));
            }

            if (states.TryGetValue(stateType.FullName, out IState<T> state))
            {
                return state;
            }

            return null;
        }

        /// <summary>
        /// 抛出有限状态机事件。
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="eventId">事件编号。</param>
        public void FireEvent(object sender, int eventId)
        {
            if (CurrentState == null)
            {
                throw new Exception("Current state is invalid.");
            }

            CurrentState.OnEvent(this, sender, eventId, null);
        }

        /// <summary>
        /// 抛出有限状态机事件。
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="eventId">事件编号。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void FireEvent(object sender, int eventId, object userData)
        {
            if (CurrentState == null)
            {
                throw new Exception("Current state is invalid.");
            }

            CurrentState.OnEvent(this, sender, eventId, userData);
        }

        /// <summary>
        /// 是否存在有限状态机数据。
        /// </summary>
        /// <param name="name">有限状态机数据名称。</param>
        /// <returns>有限状态机数据是否存在。</returns>
        public bool HasData(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Data name is invalid.");
            }

            return datas.ContainsKey(name);
        }

        /// <summary>
        /// 获取有限状态机数据。
        /// </summary>
        /// <typeparam name="TData">要获取的有限状态机数据的类型。</typeparam>
        /// <param name="name">有限状态机数据名称。</param>
        /// <returns>要获取的有限状态机数据。</returns>
        public TData GetData<TData>(string name) where TData : IArgs
        {
            return (TData)GetData(name);
        }

        /// <summary>
        /// 获取有限状态机数据。
        /// </summary>
        /// <param name="name">有限状态机数据名称。</param>
        /// <returns>要获取的有限状态机数据。</returns>
        public IArgs GetData(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Data name is invalid.");
            }

            if (datas.TryGetValue(name, out IArgs data))
            {
                return data;
            }

            return null;
        }

        /// <summary>
        /// 设置有限状态机数据。
        /// </summary>
        /// <typeparam name="TData">要设置的有限状态机数据的类型。</typeparam>
        /// <param name="name">有限状态机数据名称。</param>
        /// <param name="data">要设置的有限状态机数据。</param>
        public void SetData<TData>(string name, TData data) where TData : IArgs
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Data name is invalid.");
            }

            datas[name] = data;
        }

        /// <summary>
        /// 设置有限状态机数据。
        /// </summary>
        /// <param name="name">有限状态机数据名称。</param>
        /// <param name="data">要设置的有限状态机数据。</param>
        public void SetData(string name, IArgs data)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Data name is invalid.");
            }

            datas[name] = data;
        }

        /// <summary>
        /// 移除有限状态机数据。
        /// </summary>
        /// <param name="name">有限状态机数据名称。</param>
        /// <returns>是否移除有限状态机数据成功。</returns>
        public bool RemoveData(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Data name is invalid.");
            }

            return datas.Remove(name);
        }

        /// <summary>
        /// 有限状态机轮询。
        /// </summary>
        internal override void Update()
        {
            if (CurrentState == null)
            {
                return;
            }

            CurrentState.OnUpdate(this);
        }

        /// <summary>
        /// 关闭并清理有限状态机。
        /// </summary>
        internal override void Shutdown()
        {
            if (CurrentState != null)
            {
                CurrentState.OnLeave(this, true);
                CurrentState = null;
                currentStateTime = 0f;
            }

            foreach (KeyValuePair<string, IState<T>> state in states)
            {
                state.Value.OnDestroy(this);
            }

            states.Clear();
            datas.Clear();

            isDestroyed = true;
        }

        /// <summary>
        /// 切换当前有限状态机状态。
        /// </summary>
        /// <typeparam name="TState">要切换到的有限状态机状态类型。</typeparam>
        public void ChangeState<TState>() where TState : IState<T>
        {
            ChangeState(typeof(TState));
        }

        /// <summary>
        /// 切换当前有限状态机状态。
        /// </summary>
        /// <param name="stateType">要切换到的有限状态机状态类型。</param>
        public void ChangeState(Type stateType)
        {
            if (CurrentState == null)
            {
                throw new Exception("Current state is invalid.");
            }

            IState<T> state = GetState(stateType);
            if (state == null)
            {
                throw new Exception(string.Format("FSM '{0}' can not change state to '{1}' which is not exist.", Name, stateType.FullName));
            }

            CurrentState.OnLeave(this, false);
            currentStateTime = 0f;
            CurrentState = state;
            CurrentState.OnEnter(this);
        }
    }
}

