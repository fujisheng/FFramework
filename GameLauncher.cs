using Framework.Module;
using UnityEngine;

namespace Framework
{
    public class GameLauncher : MonoBehaviour
    {
        private void Awake()
        {
            Application.targetFrameRate = 144;

            ModuleManager.Load();
        }

        private void Update()
        {
            ModuleManager.Update();
        }

        private void LateUpdate()
        {
            ModuleManager.LateUpdate();
        }

        private void FixedUpdate()
        {
            ModuleManager.FixedUpdate();
        }

        private void OnDestroy()
        {
            ModuleManager.TearDown();
        }

        private void OnApplicationFocus(bool focus)
        {
            ModuleManager.ApplicationFocus(focus);
        }

        private void OnApplicationPause(bool pause)
        {
            ModuleManager.ApplicationPause(pause);
        }

        private void OnApplicationQuit()
        {
            ModuleManager.ApplicationQuit();
        }
    }
}

