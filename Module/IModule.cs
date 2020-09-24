using System.Threading.Tasks;

namespace Framework.Module
{
    public interface IModule
    {
        Task OnLoad();
        Task OnInit();
        void OnUpdate();
        void OnLateUpdate();
        void OnFixedUpdate();
        void OnTearDown();
        void OnApplicationFocus(bool focus);
        void OnApplicationPause(bool pause);
        void OnApplicationQuit();
    }
}