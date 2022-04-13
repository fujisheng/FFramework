using UnityEngine;

namespace Framework.Service
{
    [AddComponentMenu("")]
    class ServicesEntry : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Initialize()
        {
            if (FindObjectOfType<ServicesEntry>() != null)
            {
                return;
            }
            var obj = new GameObject("[ServicesEntry]");
            obj.AddComponent<ServicesEntry>();
            obj.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.NotEditable;
            DontDestroyOnLoad(obj);
        }

        private void Awake()
        {
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

