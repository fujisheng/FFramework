using UnityEngine;

namespace Framework
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    if (instance == null)
                    {
                        instance = new GameObject($"[{typeof(T).Name}]").AddComponent<T>();
                    }

                    instance.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.NotEditable;
                }
                return instance;
            }
        }
        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }
}