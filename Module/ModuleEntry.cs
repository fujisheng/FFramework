using UnityEngine;

namespace Framework.Module
{
    [AddComponentMenu("")]
    internal class ModuleEntry: MonoBehaviour
    {
        private void Awake()
        {
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

