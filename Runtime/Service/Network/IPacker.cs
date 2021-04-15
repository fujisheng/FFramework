using Framework.Service.ObjectPool;

namespace Framework.Service.Network
{
    public interface IPacker
    {
        void SetMessagePool(IObjectPool<IMessage> messagePool);
        byte[] Pack(IMessage message);
        IMessage Unpack(byte[] bytes);
        void Close();
    }
}