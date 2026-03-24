namespace Framework.EntityComponent
{
    /// <summary>
    /// 实体
    /// </summary>
    public class Entity
    {
        readonly EntityManager entityManager;
        public readonly int Id;

        internal Entity(EntityManager entityManager, int id)
        {
            this.entityManager = entityManager;
            this.Id = id;
        }

        public T GetComponent<T>() where T : IComponent
        {
            return entityManager.GetComponent<T>(Id);
        }

        public void AddComponent<T>(T component) where T : IComponent
        {
            entityManager.AddComponent(Id, component);
        }

        public void RemoveComponent<T>() where T : IComponent
        {
            entityManager.RemoveComponent<T>(Id);
        }

        public override string ToString()
        {
            return $"Entity:{Id}";
        }
    }
}