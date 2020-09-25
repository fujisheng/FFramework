using System.Threading.Tasks;

namespace Framework.Module
{
    internal abstract class Module 
    {
        internal virtual async Task OnLoad() { }
        internal virtual async Task OnInit() { }
        internal virtual void OnUpdate() { }
        internal virtual void OnLateUpdate() { }
        internal virtual void OnFixedUpdate() { }
        internal virtual void OnTearDown() { }
        internal virtual void OnApplicationFocus(bool focus) { }
        internal virtual void OnApplicationPause(bool pause) { }
        internal virtual void OnApplicationQuit() { }
    }
}