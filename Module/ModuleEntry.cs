using UnityEngine;

namespace Framework.Module
{
    [AddComponentMenu("")]
    internal class ModuleEntry: MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Application.lowMemory += OnLowMemory;
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
            Application.lowMemory -= OnLowMemory;
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

        private void OnLowMemory()
        {
            ModuleManager.OnLowMemory();
        }
    }
}

