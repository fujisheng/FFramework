namespace Framework.Service.Network
{
    public interface IPacket
    {
        int Id { get; set; }
        byte[] Bytes { get; set; }
        int Length { get; set; }
        void Release();
    }
}