using System.Threading.Tasks;

namespace Framework.Module
{
    public abstract class ModuleBase : IModule
    {
        public virtual async Task OnLoad() { }
        public virtual async Task OnInit() { }
        public virtual void OnUpdate() { }
        public virtual void OnLateUpdate() { }
        public virtual void OnFixedUpdate() { }
        public virtual void OnTearDown() { }
        public virtual void OnApplicationFocus(bool focus) { }
        public virtual void OnApplicationPause(bool pause) { }
        public virtual void OnApplicationQuit() { }
    }
}