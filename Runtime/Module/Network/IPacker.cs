using Framework.Module.ObjectPool;

namespace Framework.Module.Network
{
    public interface IPacker
    {
        void SetMessagePool(IObjectPool<IMessage> messagePool);
        byte[] Pack(IMessage message);
        IMessage Unpack(byte[] bytes);
        void Close();
    }
}