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

        static ModuleManager instance;
        static GameObject moduleEntry;

        ModuleManager() { }

        public static ModuleManager Instance
        {
            get
            {
                if(instance == null)
                {
                    moduleEntry = new GameObject("[ModuleEntry]");
                    moduleEntry.AddComponent<ModuleEntry>();
                    instance = new ModuleManager();
                }
                return instance;
            }
        }

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
        /// 创建模块 如果这个模块有依赖会先创建其依赖
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        Module CreateModule(string moduleName)
        {
            Type moduleType = AssemblyUtility.GetType(moduleName);
            if(moduleType == null)
            {
                throw new Exception($"can't found this type {moduleName}");
            }

            var module = Activator.CreateInstance(moduleType, true);
            foreach (var mod in loadedModules)
            {
                if (mod.GetType().FullName == moduleType.FullName)
                {
                    return mod;
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
                    string dependName = string.Format("{0}.{1}", depend.Namespace, depend.Name.Substring(1));
                    CreateModule(dependName);
                }
            }

            Debug.Log($"<color=blue>create module=>{module.GetType().FullName}</color>");
            loadedModules.Add(module as Module);
            return module as Module;
        }

        /// <summary>
        /// 获取游戏框架模块。
        /// </summary>
        /// <typeparam name="T">要获取的游戏框架模块类型。</typeparam>
        /// <returns>要获取的游戏框架模块。</returns>
        /// <remarks>如果要获取的游戏框架模块不存在，则自动创建该游戏框架模块和其依赖。</remarks>
        public T GetModule<T>() where T : class
        {
            Type interfaceType = typeof(T);
            if (!interfaceType.IsInterface)
            {
                throw new Exception($"You must get module by interface, but '{interfaceType.FullName}' is not.");
            }

            if (!interfaceType.FullName.StartsWith("Framework.Module", StringComparison.Ordinal))
            {
                throw new Exception($"You must get a Framework module, but '{interfaceType.FullName}' is not.");
            }

            string moduleName = string.Format("{0}.{1}", interfaceType.Namespace, interfaceType.Name.Substring(1));
            Type moduleType = Type.GetType(moduleName);
            if (moduleType == null)
            {
                throw new Exception($"Can not find module type '{moduleName}'.");
            }

            return GetModule(moduleType) as T;
        }

        // 获取游戏框架模块。如果要获取的游戏框架模块不存在，则自动创建该游戏框架模块
        Module GetModule(Type moduleType)
        {
            foreach (Module module in loadedModules)
            {
                if (module.GetType() == moduleType)
                {
                    return module;
                }
            }

            return CreateModule(moduleType.FullName);
        }



        internal void Update()
        {
            for(int i = 0; i < loadedModules.Count; i++)
            {
                loadedModules[i].OnUpdate();
            }
        }

        internal void LateUpdate()
        {
            for(int i = 0; i < loadedModules.Count; i++)
            {
                loadedModules[i].OnLateUpdate();
            }
        }

        internal void FixedUpdate()
        {
            for (int i = 0; i < loadedModules.Count; i++)
            {
                loadedModules[i].OnFixedUpdate();
            }
        }

        internal void TearDown()
        {
            for (int i = loadedModules.Count - 1; i >= 0; i--)
            {
                loadedModules[i].OnTearDown();
            }
        }

        internal void ApplicationFocus(bool focus)
        {
            for (int i = 0; i < loadedModules.Count; i++)
            {
                loadedModules[i].OnApplicationFocus(focus);
            }
        }

        internal void ApplicationPause(bool pause)
        {
            for (int i = 0; i < loadedModules.Count; i++)
            {
                loadedModules[i].OnApplicationPause(pause);
            }
        }

        internal void ApplicationQuit()
        {
            for (int i = 0; i < loadedModules.Count; i++)
            {
                loadedModules[i].OnApplicationQuit();
            }
        }
    }
}
