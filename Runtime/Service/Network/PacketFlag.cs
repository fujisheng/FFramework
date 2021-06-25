namespace Framework.Service.Network
{
    public enum PacketFlag
    {
        /// <summary>
        /// 加密
        /// </summary>
        Encrypt = 1 << 0,

        /// <summary>
        /// 压缩
        /// </summary>
        Compress = 1 << 1,

        /// <summary>
        /// 不解析
        /// </summary>
        NoParse = 1 << 7,
    }
}
