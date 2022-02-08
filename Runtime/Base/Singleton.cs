using System.Reflection;

namespace Framework
{
    public abstract class Singleton<T> where T : class
    {
        protected static T instance;
        public static T Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = System.Activator.CreateInstance(typeof(T)) as T;
                    typeof(T).GetMethod("OnConstructor", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static)?.Invoke(instance, null);
                }
                
                return instance;
            }
        }

        protected virtual void OnConstructor() { }
    }
}