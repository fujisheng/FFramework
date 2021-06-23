namespace Framework.Service.Network
{
    public class PacketPool : ObjectPool<IPacket>
    {
        public override int Size => 10;
        protected override IPacket New()
        {
            return new Packet();
        }
    }
}