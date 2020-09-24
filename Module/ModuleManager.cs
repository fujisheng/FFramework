using Framework.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Framework.Module
{
    public class ModuleManager
    {
        readonly List<IModule> loadedModules = new List<IModule>();
        readonly List<IModule> initedModules = new List<IModule>();

        static ModuleManager instance;
        public static ModuleManager Instance { get { return instance ??= new ModuleManager(); } }

        Type[] GetModuleDependency(IModule module)
        {
            var dependency = module.GetType().GetCustomAttribute<Dependency>();
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
        public void AddModule(IModule module)
        {
            foreach(var mod in loadedModules)
            {
                if(mod.GetType().Name == module.GetType().Name)
                {
                    return;
                }
            }

            var dependency = GetModuleDependency(module);
            if(dependency != null)
            {
                foreach(var depend in dependency)
                {
                    if (DependIsLoad(depend))
                    {
                        continue;
                    }
                    AddModule(InstanceFactory.CreateInstance<IModule>(depend.Name));
                }
            }

            module.OnLoad();
            Debug.Log($"<color=blue>LoadModule=>{module.GetType().Name}</color>");
            loadedModules.Add(module);
        }

        

        public void Update()
        {
            for(int i = 0; i < initedModules.Count; i++)
            {
                initedModules[i].OnUpdate();
            }
        }

        public void LateUpdate()
        {
            for(int i = 0; i < initedModules.Count; i++)
            {
                initedModules[i].OnLateUpdate();
            }
        }

        public void FixedUpdate()
        {
            for (int i = 0; i < initedModules.Count; i++)
            {
                initedModules[i].OnFixedUpdate();
            }
        }

        public void TearDown()
        {
            for (int i = initedModules.Count - 1; i >= 0; i--)
            {
                initedModules[i].OnTearDown();
            }
        }

        public void ApplicationFocus(bool focus)
        {
            for (int i = 0; i < initedModules.Count; i++)
            {
                initedModules[i].OnApplicationFocus(focus);
            }
        }

        public void ApplicationPause(bool pause)
        {
            for (int i = 0; i < initedModules.Count; i++)
            {
                initedModules[i].OnApplicationPause(pause);
            }
        }

        public void ApplicationQuit()
        {
            for (int i = 0; i < initedModules.Count; i++)
            {
                initedModules[i].OnApplicationQuit();
            }
        }

        /// <summary>
        /// 获取一个module
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetModule<T>()
        {
            for (int i = 0; i < initedModules.Count; i++)
            {
                var module = initedModules[i];
                if(initedModules[i] is T)
                {
                    return (T)module;
                }
            }

            return default;
        }

        public IModule GetModule(Type type)
        {
            for(int i = 0; i < initedModules.Count; i++)
            {
                var module = initedModules[i];
                if (module.GetType().IsAssignableFrom(type))
                {
                    return module;
                }
            }

            return default;
        }
    }
}
