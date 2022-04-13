using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Framework.ECS
{
    public class World
    {
        /// <summary>
        /// 世界的名字
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// 这个世界所有的系统
        /// </summary>
        List<ComponentSystem> systems;

        /// <summary>
        /// 这个世界所对应的实体管理器
        /// </summary>
        public EntityManager EntityManager { get; private set; }

        /// <summary>
        /// 这个世界是否在运行中
        /// </summary>
        public bool Running { get; private set; }

        /// <summary>
        /// 默认的世界名字
        /// </summary>
        const string DefaultWorldName = "DefaultWorld";

        /// <summary>
        /// 通过一个名字创建一个世界
        /// </summary>
        /// <param name="name"></param>
        public World(string name)
        {
            this.Name = name;
            systems = new List<ComponentSystem>();
            EntityManager = new EntityManager();
            Running = true;
        }

        /// <summary>
        /// 创建一个系统
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void CreateSystem<T>()
        {
            CreateSystem(typeof(T));
        }

        /// <summary>
        /// 创建一个系统
        /// </summary>
        /// <param name="type">系统类型</param>
        public int CreateSystem(Type type)
        {
            if (!typeof(ComponentSystem).IsAssignableFrom(type))
            {
                throw new Exception($"world {Name} create system {type} faile, {type} is not {nameof(ComponentSystem)}");
            }

            if (systems.Exists(item => item.GetType() == type))
            {
                throw new Exception($"world {Name} create system {type} faild, already has system {type}");
            }

            var system = Activator.CreateInstance(type) as ComponentSystem;
            system.Initialize(EntityManager);
            system.OnCreate();

            var beforeAttribute = type.GetCustomAttribute<UpdateBeforeAttribute>();
            var afterAttribute = type.GetCustomAttribute<UpdateAfterAttribute>();

            if (beforeAttribute != null)
            {
                var beforeTarget = beforeAttribute.target;
                var beforeIndex = systems.FindIndex(item => item.GetType() == beforeTarget);
                if(beforeIndex == -1)
                {
                    var i = CreateSystem(beforeTarget);
                    systems.Insert(i, system);
                    return i;
                }
                else
                {
                    systems.Insert(beforeIndex, system);
                    return beforeIndex;
                }
            }

            if(afterAttribute != null)
            {
                var afterTarget = afterAttribute.target;
                var afterIndex = systems.FindIndex(item => item.GetType() == afterTarget);
                if(afterIndex == -1)
                {
                    var i = CreateSystem(afterTarget);
                    if(i == systems.Count - 1)
                    {
                        systems.Add(system);
                        return systems.Count - 1;
                    }
                    else
                    {
                        systems.Insert(i + 1, system);
                        return i + 1;
                    }
                }
                else
                {
                    systems.Insert(afterIndex + 1, system);
                    return afterIndex;
                }
            }

            
            systems.Add(system);
            return systems.Count - 1;
        }

        /// <summary>
        /// 销毁一个系统
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void DestroySystem<T>()
        {
            var system = systems.FirstOrDefault(item => item is T);
            if(system == null)
            {
                throw new Exception($"world {Name} destroy system {typeof(T)} faild, dont have this system");
            }

            system.OnStopRunning();
            system.OnDestroy();
            systems.Remove(system);
        }

        /// <summary>
        /// 销毁这个世界
        /// </summary>
        public void Destroy()
        {
            foreach(var system in systems)
            {
                system.OnStopRunning();
                system.OnDestroy();
            }
            systems.Clear();
            EntityManager.DestroyEntities();
        }

        /// <summary>
        /// 开始运行世界
        /// </summary>
        public void Start()
        {
            Running = true;
            systems.ForEach(item => item.OnStartRunning());
        }

        /// <summary>
        /// 更新世界
        /// </summary>
        public void Update()
        {
            if(Running == false)
            {
                return;
            }
            systems.ForEach(item => item.OnUpdate());
        }

        /// <summary>
        /// 停止世界
        /// </summary>
        public void Stop()
        {
            Running = false;
            systems.ForEach(item => item.OnStopRunning());
        }


        /// <summary>
        /// 默认世界
        /// </summary>
        static World defaultWorld;

        /// <summary>
        /// 默认世界
        /// </summary>
        public static World Default
        {
            get
            {
                return defaultWorld ?? (defaultWorld = new World(DefaultWorldName));
            }
        }
    }
}