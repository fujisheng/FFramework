using Framework.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using FInject;

namespace Framework.Module
{
    /// <summary>
    /// 模块管理器 外部通过模块接口可以访问到对应的模块
    /// </summary>
    public static class ModuleManager
    {
        static readonly List<Module> loadedModules = new List<Module>();
        public static IModuleInjectInfo InjectInfo { get; private set; }

        static ModuleManager() 
        {
            var moduleEntry = new GameObject("[ModuleEntry]");
            moduleEntry.AddComponent<ModuleEntry>();
        }

        public static void SetInjectInfo(IModuleInjectInfo injectInfo)
        {
            InjectInfo = injectInfo;
            Injecter.Context = injectInfo.Context;
            injectInfo.Initialize();
        }

        /// <summary>
        /// 根据模块的type获取依赖的模块
        /// </summary>
        /// <param name="moduleType"></param>
        /// <returns></returns>
        static Type[] GetModuleDependency(Type moduleType)
        {
            var dependency = moduleType.GetCustomAttribute<Dependency>();
            if(dependency == null)
            {
                return null;
            }
            return dependency.dependency;
        }

        /// <summary>
        /// 判断依赖的模块是否已经加载
        /// </summary>
        /// <param name="dependType"></param>
        /// <returns></returns>
        static bool DependIsLoad(Type dependType)
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
        /// <param name="moduleName">模块的名字</param>
        /// <returns>模块</returns>
        static Module CreateModule(string moduleName)
        {
            Type moduleType = AssemblyUtility.GetType(moduleName);
            if(moduleType == null)
            {
                throw new Exception($"can't found this type {moduleName}");
            }

            var module = Injecter.CreateInstance(moduleType);
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
        public static T GetModule<T>() where T : class
        {
            if(InjectInfo == null)
            {
                throw new NullReferenceException($"You must set Injecter first");
            }

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

        /// <summary>
        /// 根据模块type获取模块 如果没有加载则会尝试创建并加载
        /// </summary>
        /// <param name="moduleType">模块类型</param>
        /// <returns>模块</returns>
        static Module GetModule(Type moduleType)
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

        /// <summary>
        /// 模块Update
        /// </summary>
        internal static void Update()
        {
            for(int i = 0; i < loadedModules.Count; i++)
            {
                loadedModules[i].OnUpdate();
            }
        }

        /// <summary>
        /// 模块LateUpdate
        /// </summary>
        internal static void LateUpdate()
        {
            for(int i = 0; i < loadedModules.Count; i++)
            {
                loadedModules[i].OnLateUpdate();
            }
        }

        /// <summary>
        /// 模块FixedUpdate
        /// </summary>
        internal static void FixedUpdate()
        {
            for (int i = 0; i < loadedModules.Count; i++)
            {
                loadedModules[i].OnFixedUpdate();
            }
        }

        /// <summary>
        /// 模块关闭
        /// </summary>
        internal static void TearDown()
        {
            for (int i = loadedModules.Count - 1; i >= 0; i--)
            {
                loadedModules[i].OnTearDown();
            }
        }

        /// <summary>
        /// 程序聚焦
        /// </summary>
        /// <param name="focus"></param>
        internal static void ApplicationFocus(bool focus)
        {
            for (int i = 0; i < loadedModules.Count; i++)
            {
                loadedModules[i].OnApplicationFocus(focus);
            }
        }

        /// <summary>
        /// 程序暂停
        /// </summary>
        /// <param name="pause"></param>
        internal static void ApplicationPause(bool pause)
        {
            for (int i = 0; i < loadedModules.Count; i++)
            {
                loadedModules[i].OnApplicationPause(pause);
            }
        }

        /// <summary>
        /// 程序退出
        /// </summary>
        internal static void ApplicationQuit()
        {
            for (int i = 0; i < loadedModules.Count; i++)
            {
                loadedModules[i].OnApplicationQuit();
            }
        }

        /// <summary>
        /// 低内存
        /// </summary>
        internal static void OnLowMemory()
        {
            for(int i = 0; i < loadedModules.Count; i++)
            {
                loadedModules[i].OnLowMemory();
            }
        }
    }
}
