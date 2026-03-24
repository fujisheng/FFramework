using UnityEngine;

namespace Framework.Module
{
    [AddComponentMenu("")]
    public class ModuleEntry : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Initialize()
        {
            if (FindObjectOfType<ModuleEntry>() != null)
            {
                return;
            }
            var obj = new GameObject("[ModuleEntry]");
            obj.AddComponent<ModuleEntry>();
            obj.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.NotEditable;
            DontDestroyOnLoad(obj);
        }

        void Awake()
        {
            Application.lowMemory += OnLowMemory;
        }

        void Update()
        {
            ModuleManager.Update();
        }

        void LateUpdate()
        {
            ModuleManager.LateUpdate();
        }

        void FixedUpdate()
        {
            ModuleManager.FixedUpdate();
        }

        void OnDestroy()
        {
            ModuleManager.TearDown();
            Application.lowMemory -= OnLowMemory;
        }

        void OnApplicationFocus(bool focus)
        {
            ModuleManager.ApplicationFocus(focus);
        }

        void OnApplicationPause(bool pause)
        {
            ModuleManager.ApplicationPause(pause);
        }

        void OnApplicationQuit()
        {
            ModuleManager.ApplicationQuit();
        }

        void OnLowMemory()
        {
            ModuleManager.OnLowMemory();
        }
    }
}

