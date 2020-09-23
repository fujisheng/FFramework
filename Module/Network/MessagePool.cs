namespace Framework.Module.Network
{
    internal class MessagePool : ObjectPool<IMessage>
    {
        public override int Size => 10;
        public override IMessage New()
        {
            return new Message();
        }
    }
}