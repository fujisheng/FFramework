using System.Collections.Generic;

namespace Framework.Module.Loop
{
    /// <summary>
    /// 循环管理器
    /// </summary>
    internal sealed class LoopModule : Module, ILoopModule
    {
        List<System.Action<float>> updateList = new List<System.Action<float>>();
        List<System.Action<float>> lateUpdateList = new List<System.Action<float>>();
        List<System.Action<float>> fixedUpdateList = new List<System.Action<float>>();

        /// <summary>
        /// 添加一个Update
        /// </summary>
        /// <param name="update">对应的方法</param>
        public void AddUpdate(System.Action<float> update)
        {
            if (updateList.Exists((_update) => _update == update))
            {
                return;
            }

            updateList.Add(update);
        }

        /// <summary>
        /// 移除一个Update
        /// </summary>
        /// <param name="update">对应的方法</param>
        public void RemoveUpdate(System.Action<float> update)
        {
            if (updateList.Exists((_update) => _update == update))
            {
                updateList.Remove(update);
            }
        }

        /// <summary>
        /// 添加一个LateUpdate
        /// </summary>
        /// <param name="lateUpdate">对应的方法</param>
        public void AddLateUpdate(System.Action<float> lateUpdate)
        {
            if (lateUpdateList.Exists((_update) => _update == lateUpdate))
            {
                return;
            }

            lateUpdateList.Add(lateUpdate);
        }

        /// <summary>
        /// 移除一个LateUpdate
        /// </summary>
        /// <param name="lateUpdate">对应的方法</param>
        public void RemoveLateUpdate(System.Action<float> lateUpdate)
        {
            if (lateUpdateList.Exists((_update) => _update == lateUpdate))
            {
                lateUpdateList.Remove(lateUpdate);
            }
        }

        /// <summary>
        /// 添加一个FixedUpdate
        /// </summary>
        /// <param name="fixedUpdate">对应的方法</param>
        public void AddFixedUpdate(System.Action<float> fixedUpdate)
        {
            if (fixedUpdateList.Exists((_update) => _update == fixedUpdate))
            {
                return;
            }

            fixedUpdateList.Add(fixedUpdate);
        }

        /// <summary>
        /// 移除一个FixedUpdate
        /// </summary>
        /// <param name="fixedUpdate">对应的方法</param>
        public void RemoveFixedUpdate(System.Action<float> fixedUpdate)
        {
            if (fixedUpdateList.Exists((_update) => _update == fixedUpdate))
            {
                fixedUpdateList.Remove(fixedUpdate);
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        internal override void OnUpdate()
        {
            for (int i = 0; i < updateList.Count; i++)
            {
                updateList[i].Invoke(UnityEngine.Time.deltaTime);
            }
        }

        /// <summary>
        /// LateUpdate
        /// </summary>
        internal override void OnLateUpdate()
        {
            for (int i = 0; i < lateUpdateList.Count; i++)
            {
                lateUpdateList[i].Invoke(UnityEngine.Time.deltaTime);
            }
        }

        /// <summary>
        /// FixedUpdate
        /// </summary>
        internal override void OnFixedUpdate()
        {
            for(int i = 0; i < fixedUpdateList.Count; i++)
            {
                fixedUpdateList[i].Invoke(UnityEngine.Time.fixedDeltaTime);
            }
        }

        /// <summary>
        /// 关闭的时候
        /// </summary>
        internal override void OnTearDown()
        {
            updateList.Clear();
            fixedUpdateList.Clear();
            lateUpdateList.Clear();
        }
    }

}
