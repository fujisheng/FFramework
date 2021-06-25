namespace Framework.Service.Network
{
    public interface INetworkBCCHelper
    {
        /// <summary>
        /// 计算bcc
        /// </summary>
        /// <param name="buffer">数据buffer</param>
        /// <param name="offset">偏移</param>
        /// <param name="length">数据长度</param>
        /// <returns>bcc</returns>
        byte Calculation(byte[] buffer, int offset, int length);

        /// <summary>
        /// 检查bcc
        /// </summary>
        /// <param name="buffer">数据buffer</param>
        /// <param name="offset">偏移</param>
        /// <param name="length">数据长度</param>
        /// <param name="bcc">bcc</param>
        /// <returns>是否校验成功</returns>
        bool Check(byte[] buffer, int offset, int length, byte bcc);
    }
}