using System;
using System.Collections.Generic;

namespace Framework.EntityComponent
{
    public interface IEntityUpdater
    {
        void AddUpdate(Action<float> action);
        void RemoveUpdate(Action<float> action);
    }

    /// <summary>
    /// 实体管理器
    /// </summary>
    public class EntityManager
    {
        #region 组件唯一id
        struct ComponentUniqueKey : IEquatable<ComponentUniqueKey>
        {
            public int entityId;
            public Type componentType;

            public bool Equals(ComponentUniqueKey other)
            {
                return entityId == other.entityId && componentType == other.componentType;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(entityId, componentType);
            }
        }
        #endregion

        /// <summary>
        /// 实体id映射表
        /// </summary>
        readonly Dictionary<int, Entity> entities = new Dictionary<int, Entity>();

        /// <summary>
        /// 组件映射表
        /// </summary>
        readonly Dictionary<ComponentUniqueKey, IComponent> components = new 
Dictionary<ComponentUniqueKey, IComponent>();

        /// <summary>
        /// 组件类型到实体id映射表
        /// </summary>
        readonly Dictionary<Type, HashSet<int>> componentTypeToEntityIds = new Dictionary<Type, HashSet<int>>();

        /// <summary>
        /// 下一个实体id
        /// </summary>
        int nextEntityId = 0;

        readonly IEntityUpdater updater;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="updater">更新器  如果没有将不会调用Component的更新接口</param>
        public EntityManager(IEntityUpdater updater = null)
        {
            this.updater = updater;
            updater.AddUpdate(Update);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="deltaTime"></param>
        void Update(float deltaTime)
        {
            foreach (var component in components.Values)
            {
                component.OnUpdate(deltaTime);
            }
        }

        /// <summary>
        /// 创建一个实体
        /// </summary>
        /// <returns></returns>
        public Entity CreateEntity()
        {
            var id = nextEntityId++;
            var entity = new Entity(this, id);
            entities[id] = entity;
            return entity;
        }

        /// <summary>
        /// 销毁一个实体
        /// </summary>
        /// <param name="entity">要销毁的实体</param>
        public void DestroyEntity(Entity entity)
        {
            var id = entity.Id;
            entities.Remove(id);

            var keysToRemove = new List<ComponentUniqueKey>();
            foreach (var key in components.Keys)
            {
                if (key.entityId == id)
                {
                    keysToRemove.Add(key);
                }
            }

            foreach (var key in keysToRemove)
            {
                InternalRemoveComponent(key);
            }
        }

        /// <summary>
        /// 获取一个组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="entityId">实体id</param>
        /// <returns></returns>
        internal T GetComponent<T>(int entityId) where T : IComponent
        {
            var key = new ComponentUniqueKey { entityId = entityId, componentType = typeof(T) };
            if (components.TryGetValue(key, out var component))
            {
                return (T)component;
            }
            return default;
        }

        /// <summary>
        /// 添加一个组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="entityId">实体id</param>
        /// <param name="component">组件</param>
        internal void AddComponent<T>(int entityId, T component) where T : IComponent
        {
            var key = new ComponentUniqueKey { entityId = entityId, componentType = typeof(T) };
            components[key] = component;

            if (!componentTypeToEntityIds.TryGetValue(typeof(T), out var entityIds))
            {
                entityIds = new HashSet<int>();
                componentTypeToEntityIds[typeof(T)] = entityIds;
            }
            entityIds.Add(entityId);

            component.OnCreate();
        }

        /// <summary>
        /// 移除一个组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="entityId">实体id</param>
        internal void RemoveComponent<T>(int entityId) where T : IComponent
        {
            var key = new ComponentUniqueKey { entityId = entityId, componentType = typeof(T) };
            InternalRemoveComponent(key);
        }

        /// <summary>
        /// 根据组件唯一id移除组件
        /// </summary>
        /// <param name="key">组件</param>
        void InternalRemoveComponent(ComponentUniqueKey key)
        {
            if (components.TryGetValue(key, out var component))
            {
                component.OnDestroy();
                components.Remove(key);
            }

            if (componentTypeToEntityIds.TryGetValue(key.componentType, out var entityIds))
            {
                entityIds.Remove(key.entityId);
                if (entityIds.Count == 0)
                {
                    componentTypeToEntityIds.Remove(key.componentType);
                }
            }
        }

        /// <summary>
        /// 获取所有拥有指定组件的实体
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        public List<Entity> GetEntitiesWithComponent<T>() where T : IComponent
        {
            var result = new List<Entity>();
            if (componentTypeToEntityIds.TryGetValue(typeof(T), out var entityIds))
            {
                foreach (var entityId in entityIds)
                {
                    result.Add(entities[entityId]);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取所有拥有指定组件的实体
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="result">拥有指定组件的实体</param>
        public void GetEntitiesWithComponent<T>(in List<Entity> result) where T : IComponent
        {
            result.Clear();
            if (componentTypeToEntityIds.TryGetValue(typeof(T), out var entityIds))
            {
                foreach (var entityId in entityIds)
                {
                    result.Add(entities[entityId]);
                }
            }
        }

        /// <summary>
        /// 销毁实体管理器
        /// </summary>
        public void Destroy()
        {
            updater?.RemoveUpdate(Update);

            foreach (var entity in entities.Values)
            {
                DestroyEntity(entity);
            }
            entities.Clear();
            components.Clear();
            componentTypeToEntityIds.Clear();
        }
    }
}