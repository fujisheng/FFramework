﻿namespace Framework
{
    public abstract class Singleton<T> where T : new ()
    {
        static T instance;
        public static T Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new T();
                }

                return instance;
            }
        }
    }
}