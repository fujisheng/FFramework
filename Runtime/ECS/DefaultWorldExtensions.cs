﻿namespace Framework.ECS
{
    /// <summary>
    /// 默认世界扩展
    /// </summary>
    public static class DefaultWorldExtensions
    {
        /// <summary>
        /// 为实体添加一个组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="component">组件数据</param>
        public static void Add<T>(this Entity entity, T component) where T : struct, IComponent
        {
            World.Default.EntityManager.AddComponent<T>(entity, component);
        }

        /// <summary>
        /// 移除一个实体上的某个组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="entity">实体</param>
        public static void Remove<T>(this Entity entity) where T : struct, IComponent
        {
            World.Default.EntityManager.RemoveComponent<T>(entity);
        }

        /// <summary>
        /// 替换一个实体上的某个组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="component">组件数据</param>
        public static void Replace<T>(this Entity entity, T component) where T : struct, IComponent
        {
            World.Default.EntityManager.ReplaceComponent<T>(entity, component);
        }

        /// <summary>
        /// 是否拥有某个组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="entity">实体</param>
        public static bool Has<T>(this Entity entity) where T : struct, IComponent
        {
            return World.Default.EntityManager.HasComponent<T>(entity);
        }

        /// <summary>
        /// 获取某个组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public static T Get<T>(this Entity entity) where T : struct, IComponent
        {
            return World.Default.EntityManager.GetComponent<T>(entity);
        }
    }
}