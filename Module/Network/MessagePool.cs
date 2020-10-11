namespace Framework.Module.Network
{
    internal class MessagePool : ObjectPool<IMessage>
    {
        public override int Size => 10;
        protected override IMessage New()
        {
            return new Message();
        }
    }
}