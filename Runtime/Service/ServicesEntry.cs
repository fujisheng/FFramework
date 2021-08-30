using UnityEngine;

namespace Framework.Service
{
    [AddComponentMenu("")]
    internal class ServicesEntry : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Application.lowMemory += OnLowMemory;
        }

        private void Update() => Services.Update();

        private void LateUpdate() => Services.LateUpdate();

        private void FixedUpdate() => Services.FixedUpdate();

        private void OnDestroy()
        {
            Services.TearDown();
            Application.lowMemory -= OnLowMemory;
        }

        private void OnApplicationFocus(bool focus) => Services.ApplicationFocus(focus);

        private void OnApplicationPause(bool pause) => Services.ApplicationPause(pause);

        private void OnApplicationQuit() => Services.ApplicationQuit();

        private void OnLowMemory() => Services.OnLowMemory();
    }
}

