namespace Framework
{
    public abstract class Singleton<T> where T : class
    {
        static readonly object lockObject = new object();
        static T instance;

        public static bool HasInstance => instance != null;

        public static T TryGetInstance()
        {
            return instance;
        }

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = System.Activator.CreateInstance(typeof(T), true) as T;
                        }
                    }
                }
                return instance;
            }
        }
    }
}
