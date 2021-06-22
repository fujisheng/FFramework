using Framework.Service.ObjectPool;

namespace Framework.Service.Network
{
    public interface IPacker
    {
        void SetMessagePool(IObjectPool<IPacket> messagePool);
        byte[] Pack(IPacket message);
        IPacket Unpack(byte[] bytes);
        void Close();
    }
}