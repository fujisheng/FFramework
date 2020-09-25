using Framework.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Framework.Module
{
    public class ModuleManager
    {
        readonly List<Module> loadedModules = new List<Module>();
        readonly List<Module> initedModules = new List<Module>();

        static ModuleManager instance;
        public static ModuleManager Instance { get { return instance ??= new ModuleManager(); } }

        Type[] GetModuleDependency(Type moduleType)
        {
            var dependency = moduleType.GetCustomAttribute<Dependency>();
            if(dependency == null)
            {
                return null;
            }
            return dependency.dependency;
        }

        bool DependIsLoad(Type dependType)
        {
            foreach(var modules in loadedModules)
            {
                if (modules.GetType().IsAssignableFrom(dependType))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 添加一个模块 同一种类型的模块只允许添加一个 同时如果这个模块有依赖会自动添加依赖
        /// </summary>
        /// <param name="module"></param>
        public void AddModule<T>()
        {
            var moduleType = typeof(T);
            AddModule(InstanceFactory.CreateInstance<Module>(moduleType.Name));
        }

        void AddModule(Module module)
        {
            var moduleType = module.GetType();
            foreach (var mod in loadedModules)
            {
                if (mod.GetType().Name == moduleType.Name)
                {
                    return;
                }
            }

            var dependency = GetModuleDependency(moduleType);
            if (dependency != null)
            {
                foreach (var depend in dependency)
                {
                    if (DependIsLoad(depend))
                    {
                        continue;
                    }
                    AddModule(InstanceFactory.CreateInstance<Module>(depend.Name));
                }
            }

            module.OnLoad();
            Debug.Log($"<color=blue>LoadModule=>{module.GetType().Name}</color>");
            loadedModules.Add(module);
        }

        /// <summary>
        /// 获取一个module
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetModule<T>() where T : class
        {
            for (int i = 0; i < initedModules.Count; i++)
            {
                var module = initedModules[i];
                if (initedModules[i] is T)
                {
                    return module as T;
                }
            }

            return default;
        }



        internal void Update()
        {
            for(int i = 0; i < initedModules.Count; i++)
            {
                initedModules[i].OnUpdate();
            }
        }

        internal void LateUpdate()
        {
            for(int i = 0; i < initedModules.Count; i++)
            {
                initedModules[i].OnLateUpdate();
            }
        }

        internal void FixedUpdate()
        {
            for (int i = 0; i < initedModules.Count; i++)
            {
                initedModules[i].OnFixedUpdate();
            }
        }

        internal void TearDown()
        {
            for (int i = initedModules.Count - 1; i >= 0; i--)
            {
                initedModules[i].OnTearDown();
            }
        }

        internal void ApplicationFocus(bool focus)
        {
            for (int i = 0; i < initedModules.Count; i++)
            {
                initedModules[i].OnApplicationFocus(focus);
            }
        }

        internal void ApplicationPause(bool pause)
        {
            for (int i = 0; i < initedModules.Count; i++)
            {
                initedModules[i].OnApplicationPause(pause);
            }
        }

        internal void ApplicationQuit()
        {
            for (int i = 0; i < initedModules.Count; i++)
            {
                initedModules[i].OnApplicationQuit();
            }
        }
    }
}
