namespace Framework.Service.Network
{
    internal class MessagePool : ObjectPool<IPacket>
    {
        public override int Size => 10;
        protected override IPacket New()
        {
            return new Message();
        }
    }
}