namespace Framework.EntityComponent
{
    /// <summary>
    /// 组件
    /// </summary>
    public interface IComponent
    {
        /// <summary>
        /// 创建时调用
        /// </summary>
        void OnCreate();

        /// <summary>
        /// 更新时调用
        /// </summary>
        void OnUpdate(float deltaTime);

        /// <summary>
        /// 销毁时调用
        /// </summary>
        void OnDestroy();
    }

    /// <summary>
    /// 组件
    /// </summary>
    public abstract class Component : IComponent
    {
        public Entity Entity { get; internal set; }

        void IComponent.OnCreate() => OnCreate();

        protected virtual void OnCreate() { }

        void IComponent.OnDestroy() => OnDestroy();

        protected virtual void OnDestroy() { }

        void IComponent.OnUpdate(float deltaTime) => OnUpdate(deltaTime);

        protected virtual void OnUpdate(float deltaTime) { }
    }
}