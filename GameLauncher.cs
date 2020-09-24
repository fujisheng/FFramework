using Framework.Module;
using UnityEngine;

namespace Framework
{
    public class GameLauncher : MonoBehaviour
    {
        private void Awake()
        {
            Application.targetFrameRate = 144;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            ModuleManager.Instance.Update();
        }

        private void LateUpdate()
        {
            ModuleManager.Instance.LateUpdate();
        }

        private void FixedUpdate()
        {
            ModuleManager.Instance.FixedUpdate();
        }

        private void OnDestroy()
        {
            ModuleManager.Instance.TearDown();
        }

        private void OnApplicationFocus(bool focus)
        {
            ModuleManager.Instance.ApplicationFocus(focus);
        }

        private void OnApplicationPause(bool pause)
        {
            ModuleManager.Instance.ApplicationPause(pause);
        }

        private void OnApplicationQuit()
        {
            ModuleManager.Instance.ApplicationQuit();
        }
    }
}

