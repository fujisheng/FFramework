namespace Framework.Module.ObjectPool
{
    public class GameObjectPoolPool : ObjectPool<GameObjectPool>
    {
        public override int Size => 10;
        public override GameObjectPool New()
        {
            return new GameObjectPool();
        }
    }
}

