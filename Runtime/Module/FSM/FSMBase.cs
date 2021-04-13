using System;

namespace Framework.Module.FSM
{
    public abstract class FSMBase
    {
        public FSMBase() : this(null) { }

        public FSMBase(string name)
        {
            this.Name = name ?? string.Empty;
        }

        public string Name { get; }
        public abstract Type OwnerType{ get; }
        public abstract int StateCount{ get; }
        public abstract bool IsRunning{get; }
        public abstract bool IsDestroyed{ get; }
        public abstract string CurrentStateName{ get; }
        public abstract float CurrentStateTime{ get; }
        internal abstract void Update();
        internal abstract void Shutdown();
    }
}
