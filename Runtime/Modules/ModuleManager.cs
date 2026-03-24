using Framework.IoC;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Framework.Module
{
    /// <summary>
    /// 模块管理器 外部通过服务接口可以访问到对应的服务
    /// </summary>
    public static class ModuleManager
    {
        static readonly List<Module> loadedModules = new List<Module>();
        public static IModulesInjectInfo InjectInfo { get; private set; }

        /// <summary>
        /// 设置注入配置
        /// </summary>
        /// <param name="injectInfo">注入配置</param>
        public static void SetInjectInfo(IModulesInjectInfo injectInfo)
        {
            InjectInfo = injectInfo;
            Injector.Container = injectInfo.container;
            injectInfo.Initialize();

            CreateDefaultModule();
        }

        /// <summary>
        /// 创建默认服务
        /// </summary>
        static void CreateDefaultModule()
        {
            foreach(var type in Utility.Assembly.GetTypes())
            {
                if(type.GetCustomAttribute<DefaultModuleDependencyAttribute>() != null)
                {
                    CreateModule(type.FullName);
                }
            }
        }

        /// <summary>
        /// 根据服务的type获取依赖的服务
        /// </summary>
        /// <param name="moduleType"></param>
        /// <returns></returns>
        static Type[] GetModuleDependencies(Type moduleType)
        {
            var dependencies = moduleType.GetCustomAttribute<DependenciesAttribute>();
            if(dependencies == null)
            {
                return null;
            }
            return dependencies.dependencies;
        }

        /// <summary>
        /// 判断依赖的服务是否已经加载
        /// </summary>
        /// <param name="dependType"></param>
        /// <returns></returns>
        static bool DependencyIsLoad(Type dependType)
        {
            foreach(var module in loadedModules)
            {
                if (module.GetType().IsAssignableFrom(dependType))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 创建服务 如果这个服务有依赖会先创建其依赖
        /// </summary>
        /// <param name="moduleName">服务的名字</param>
        /// <returns>服务</returns>
        static Module CreateModule(string moduleName)
        {
            Type moduleType = Utility.Assembly.GetType(moduleName);
            Utility.Assert.IfNull(moduleType, new Exception($"can't found this type {moduleName}"));

            var module = Injector.CreateInstance(moduleType);
            foreach (var serv in loadedModules)
            {
                if (serv.GetType().FullName == moduleType.FullName)
                {
                    return serv;
                }
            }

            var dependencies = GetModuleDependencies(moduleType);
            if (dependencies != null)
            {
                foreach (var depend in dependencies)
                {
                    if (DependencyIsLoad(depend))
                    {
                        continue;
                    }
                    string dependName = string.Format("{0}.{1}", depend.Namespace, depend.Name.Substring(1));
                    CreateModule(dependName);
                }
            }

            UnityEngine.Debug.Log($"<color=blue>create module {module.GetType().FullName}</color>");
            loadedModules.Add(module as Module);
            return module as Module;
        }

        /// <summary>
        /// 获取游戏框架服务。
        /// </summary>
        /// <typeparam name="T">要获取的游戏框架服务类型。</typeparam>
        /// <returns>要获取的游戏框架服务。</returns>
        /// <remarks>如果要获取的游戏框架服务不存在，则自动创建该游戏框架服务和其依赖。</remarks>
        public static T Get<T>() where T : class
        {
            Utility.Assert.IfNull(InjectInfo, new NullReferenceException($"You must set Injecter first"));
            Type interfaceType = typeof(T);
            Utility.Assert.IfFalse(interfaceType.IsInterface, new Exception($"You must get module by interface, but '{interfaceType.FullName}' is not."));

            if (!interfaceType.FullName.StartsWith("Framework.Module", StringComparison.Ordinal))
            {
                throw new Exception($"You must get a Framework service, but '{interfaceType.FullName}' is not.");
            }

            string moduleName = string.Format("{0}.{1}", interfaceType.Namespace, interfaceType.Name.Substring(1));
            Type moduleType = Type.GetType(moduleName);
            Utility.Assert.IfNull(moduleType, new Exception($"Can not find module type '{moduleName}'."));

            return GetModule(moduleType) as T;
        }

        /// <summary>
        /// 根据服务type获取服务 如果没有加载则会尝试创建并加载
        /// </summary>
        /// <param name="moduleType">服务类型</param>
        /// <returns>服务</returns>
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

        #region 各种系统方法入口

        /// <summary>
        /// 服务Update
        /// </summary>
        internal static void Update()
        {
            for(int i= 0;i< loadedModules.Count;i++ )
            {
                loadedModules[i].OnUpdate();
            }
        }

        /// <summary>
        /// 服务LateUpdate
        /// </summary>
        internal static void LateUpdate()
        {
            for (int i = 0; i < loadedModules.Count; i++)
            {
                loadedModules[i].OnLateUpdate();
            }
        }

        /// <summary>
        /// 服务FixedUpdate
        /// </summary>
        internal static void FixedUpdate()
        {
            for (int i = 0; i < loadedModules.Count; i++)
            {
                loadedModules[i].OnFixedUpdate();
            }
        }

        /// <summary>
        /// 释放服务
        /// </summary>
        public static void Release()
        {
            for (int i = loadedModules.Count - 1; i >= 0; i--)
            {
                loadedModules[i].OnRelease();
            }
        }

        /// <summary>
        /// 服务关闭
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
            for (int i = loadedModules.Count - 1; i >= 0; i--)
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
            for (int i = loadedModules.Count - 1; i >= 0; i--)
            {
                loadedModules[i].OnApplicationPause(pause);
            }
        }

        /// <summary>
        /// 程序退出
        /// </summary>
        internal static void ApplicationQuit()
        {
            for (int i = loadedModules.Count - 1; i >= 0; i--)
            {
                loadedModules[i].OnApplicationQuit();
            }
        }

        /// <summary>
        /// 低内存
        /// </summary>
        internal static void OnLowMemory()
        {
            for (int i = loadedModules.Count - 1; i >= 0; i--)
            {
                loadedModules[i].OnLowMemory();
            }
        }
        #endregion
    }
}
