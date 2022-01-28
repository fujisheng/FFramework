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
                }
                typeof(T).GetMethod("OnConstructor")?.Invoke(instance, null);
                return instance;
            }
        }

        protected virtual void OnConstructor() { }
    }
}