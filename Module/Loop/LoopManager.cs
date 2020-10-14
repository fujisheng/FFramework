using System.Collections.Generic;

namespace Framework.Module.Loop
{
    internal sealed class LoopManager : Module, ILoopManager
    {
        List<System.Action> updateList = new List<System.Action>();
        List<System.Action> lateUpdateList = new List<System.Action>();
        List<System.Action> fixedUpdateList = new List<System.Action>();

        public void AddUpdate(System.Action update)
        {
            if (updateList.Exists((_update) => _update == update))
            {
                return;
            }

            updateList.Add(update);
        }

        public void RemoveUpdate(System.Action update)
        {
            if (updateList.Exists((_update) => _update == update))
            {
                updateList.Remove(update);
            }
        }

        public void AddLateUpdate(System.Action lateUpdate)
        {
            if (lateUpdateList.Exists((_update) => _update == lateUpdate))
            {
                return;
            }

            lateUpdateList.Add(lateUpdate);
        }

        public void RemoveLateUpdate(System.Action lateUpdate)
        {
            if (lateUpdateList.Exists((_update) => _update == lateUpdate))
            {
                lateUpdateList.Remove(lateUpdate);
            }
        }

        public void AddFixedUpdate(System.Action fixedUpdate)
        {
            if (fixedUpdateList.Exists((_update) => _update == fixedUpdate))
            {
                return;
            }

            fixedUpdateList.Add(fixedUpdate);
        }

        public void RemoveFixedUpdate(System.Action fixedUpdate)
        {
            if (fixedUpdateList.Exists((_update) => _update == fixedUpdate))
            {
                fixedUpdateList.Remove(fixedUpdate);
            }
        }

        internal override void OnUpdate()
        {
            for (int i = 0; i < updateList.Count; i++)
            {
                updateList[i].Invoke();
            }
        }

        internal override void OnLateUpdate()
        {
            for (int i = 0; i < lateUpdateList.Count; i++)
            {
                lateUpdateList[i].Invoke();
            }
        }

        internal override void OnFixedUpdate()
        {
            for(int i = 0; i < fixedUpdateList.Count; i++)
            {
                fixedUpdateList[i].Invoke();
            }
        }

        internal override void OnTearDown()
        {
            updateList.Clear();
            fixedUpdateList.Clear();
            lateUpdateList.Clear();
        }
    }

}
