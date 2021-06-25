namespace Framework.Service.Network
{
    public class PacketPool : ObjectPool<INetworkPacket>
    {
        public override int Size => 10;
        protected override INetworkPacket New()
        {
            return new Packet();
        }
    }
}