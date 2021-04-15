namespace Framework.Service.ObjectPool
{
    /// <summary>
    /// GameObjectPool的池子
    /// </summary>
    internal class GameObjectPoolPool : ObjectPool<GameObjectPool>
    {
        public override int Size => 10;
        protected override GameObjectPool New()
        {
            return new GameObjectPool();
        }
    }
}

