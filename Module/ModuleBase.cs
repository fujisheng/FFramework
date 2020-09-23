namespace Framework.Module
{
    public abstract class ModuleBase : IModule
    {
        public virtual int Priority { get { return 0; } }
        public bool LoadComplete { get; private set; }
        public bool InitComplete { get; private set; }

        public virtual void OnLoad()
        {
            LoadComplete = true;
        }

        public virtual void OnInit()
        {
            InitComplete = true;
        }

        public virtual void OnUpdate() { }

        public virtual void OnLateUpdate() { }

        public virtual void OnFixedUpdate() { }

        public virtual void OnTearDown() { }

        public virtual void OnApplicationFocus(bool focus) { }
        public virtual void OnApplicationPause(bool pause) { }
        public virtual void OnApplicationQuit() { }
    }
}