namespace Framework.ECS
{
    public abstract class ComponentSystem
    {
        protected EntityQuery Entities { get; private set; }
        protected EntityManager EntityManager { get; private set; }

        internal void Initialize(EntityManager entityManager)
        {
            Entities = new EntityQuery(entityManager);
            EntityManager = entityManager;
        }

        public virtual void OnCreate()
        {
            
        }

        public virtual void OnStartRunning()
        {
            
        }

        public virtual void OnUpdate()
        {
            
        }

        public virtual void OnStopRunning()
        {
            
        }

        public virtual void OnDestroy()
        {
            
        }
    }
}
