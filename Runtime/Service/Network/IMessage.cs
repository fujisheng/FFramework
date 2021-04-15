namespace Framework.Service.Network
{
    public interface IMessage
    {
        int Id { get; set; }
        byte[] Bytes { get; set; }
        int Length { get; set; }
        void Clear();
    }
}