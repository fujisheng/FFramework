using Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using FInject;

namespace Framework.Service
{
    /// <summary>
    /// 服务管理器 外部通过服务接口可以访问到对应的服务
    /// </summary>
    public static class Services
    {
        static readonly List<Service> loadedServices = new List<Service>();
        public static IServicesInjectInfo InjectInfo { get; private set; }

        static Services() 
        {
            if (UnityEngine.Object.FindObjectOfType<ServicesEntry>())
            {
                throw new Exception("ServiceEntry is not singlon, please check other entry");
            }
            var moduleEntry = new GameObject("[ServiceEntry]");
            moduleEntry.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.NotEditable;
            moduleEntry.AddComponent<ServicesEntry>();
        }

        /// <summary>
        /// 设置注入配置
        /// </summary>
        /// <param name="injectInfo">注入配置</param>
        public static void SetInjectInfo(IServicesInjectInfo injectInfo)
        {
            InjectInfo = injectInfo;
            Injecter.Context = injectInfo.Context;
            injectInfo.Initialize();
        }

        /// <summary>
        /// 根据服务的type获取依赖的服务
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        static Type[] GetServiceDependencies(Type serviceType)
        {
            var dependencies = serviceType.GetCustomAttribute<DependenciesAttribute>();
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
            foreach(var service in loadedServices)
            {
                if (service.GetType().IsAssignableFrom(dependType))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 创建服务 如果这个服务有依赖会先创建其依赖
        /// </summary>
        /// <param name="serviceName">服务的名字</param>
        /// <returns>服务</returns>
        static Service CreateService(string serviceName)
        {
            Type serviceType = Utility.Assembly.GetType(serviceName);
            if(serviceType == null)
            {
                throw new Exception($"can't found this type {serviceName}");
            }

            var service = Injecter.CreateInstance(serviceType);
            foreach (var serv in loadedServices)
            {
                if (serv.GetType().FullName == serviceType.FullName)
                {
                    return serv;
                }
            }

            var dependencies = GetServiceDependencies(serviceType);
            if (dependencies != null)
            {
                foreach (var depend in dependencies)
                {
                    if (DependencyIsLoad(depend))
                    {
                        continue;
                    }
                    string dependName = string.Format("{0}.{1}", depend.Namespace, depend.Name.Substring(1));
                    CreateService(dependName);
                }
            }

            UnityEngine.Debug.Log($"<color=blue>create service=>{service.GetType().FullName}</color>");
            loadedServices.Add(service as Service);
            return service as Service;
        }

        /// <summary>
        /// 获取游戏框架服务。
        /// </summary>
        /// <typeparam name="T">要获取的游戏框架服务类型。</typeparam>
        /// <returns>要获取的游戏框架服务。</returns>
        /// <remarks>如果要获取的游戏框架服务不存在，则自动创建该游戏框架服务和其依赖。</remarks>
        public static T Get<T>() where T : class
        {
            if(InjectInfo == null)
            {
                throw new NullReferenceException($"You must set Injecter first");
            }

            Type interfaceType = typeof(T);
            if (!interfaceType.IsInterface)
            {
                throw new Exception($"You must get service by interface, but '{interfaceType.FullName}' is not.");
            }

            if (!interfaceType.FullName.StartsWith("Framework.Service", StringComparison.Ordinal))
            {
                throw new Exception($"You must get a Framework service, but '{interfaceType.FullName}' is not.");
            }

            string serviceName = string.Format("{0}.{1}", interfaceType.Namespace, interfaceType.Name.Substring(1));
            Type serviceType = Type.GetType(serviceName);
            if (serviceType == null)
            {
                throw new Exception($"Can not find service type '{serviceName}'.");
            }

            return GetService(serviceType) as T;
        }

        /// <summary>
        /// 根据服务type获取服务 如果没有加载则会尝试创建并加载
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <returns>服务</returns>
        static Service GetService(Type serviceType)
        {
            foreach (Service module in loadedServices)
            {
                if (module.GetType() == serviceType)
                {
                    return module;
                }
            }

            return CreateService(serviceType.FullName);
        }

        #region 各种系统方法入口

        /// <summary>
        /// 服务Update
        /// </summary>
        internal static void Update() => loadedServices.ForEach((item) => item.OnUpdate());

        /// <summary>
        /// 服务LateUpdate
        /// </summary>
        internal static void LateUpdate() => loadedServices.ForEach((item) => item.OnLateUpdate());

        /// <summary>
        /// 服务FixedUpdate
        /// </summary>
        internal static void FixedUpdate() => loadedServices.ForEach((item) => item.OnFixedUpdate());

        /// <summary>
        /// 释放服务
        /// </summary>
        public static void Release()
        {
            for (int i = loadedServices.Count - 1; i >= 0; i--)
            {
                loadedServices[i].OnRelease();
            }
        }

        /// <summary>
        /// 服务关闭
        /// </summary>
        internal static void TearDown()
        {
            for (int i = loadedServices.Count - 1; i >= 0; i--)
            {
                loadedServices[i].OnTearDown();
            }
        }

        /// <summary>
        /// 程序聚焦
        /// </summary>
        /// <param name="focus"></param>
        internal static void ApplicationFocus(bool focus) => loadedServices.ForEach((item) => item.OnApplicationFocus(focus));

        /// <summary>
        /// 程序暂停
        /// </summary>
        /// <param name="pause"></param>
        internal static void ApplicationPause(bool pause) => loadedServices.ForEach((item) => item.OnApplicationPause(pause));

        /// <summary>
        /// 程序退出
        /// </summary>
        internal static void ApplicationQuit() => loadedServices.ForEach((item) => item.OnApplicationQuit());

        /// <summary>
        /// 低内存
        /// </summary>
        internal static void OnLowMemory() => loadedServices.ForEach((item) => item.OnLowMemory());
        #endregion
    }
}
