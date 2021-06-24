namespace Framework.Service.Network
{
    public interface IPacket
    {
        PacketHead Head { get; set; }
        byte[] Data { get; set; }
        void Release();
    }

    /// <summary>
    /// 消息头
    /// </summary>
    public struct PacketHead
    {
        /// <summary>
        /// 长度
        /// </summary>
        public readonly int length;

        /// <summary>
        /// cmd
        /// </summary>
        public readonly byte cmd;

        /// <summary>
        /// act
        /// </summary>
        public readonly byte act;

        /// <summary>
        /// 标记
        /// </summary>
        public readonly byte flags;

        /// <summary>
        /// bcc校验码
        /// </summary>
        public readonly byte bcc;

        /// <summary>
        /// 消息id
        /// </summary>
        public ushort ID
        {
            get
            {
                if(cmd == 0 || act == 0)
                {
                    throw new System.Exception("cmd or act is empty please init this head first");
                }

                return (ushort)((cmd << 8) | act);
            }
        }

        /// <summary>
        /// 构造消息头
        /// </summary>
        /// <param name="length">消息长度</param>
        /// <param name="cmd">cmd</param>
        /// <param name="act">act</param>
        /// <param name="flags">标记</param>
        /// <param name="bcc">bcc校验码</param>
        public PacketHead(int length, byte cmd, byte act, byte flags, byte bcc)
        {
            this.length = length;
            this.cmd = cmd;
            this.act = act;
            this.flags = flags;
            this.bcc = bcc;
        }

        /// <summary>
        /// 构造消息头
        /// </summary>
        /// <param name="length">消息体长度</param>
        /// <param name="id">消息id</param>
        /// <param name="flags">标记</param>
        /// <param name="bcc">bcc校验码</param>
        public PacketHead(int length, ushort id, byte flags, byte bcc)
        {
            this.length = length;
            cmd = (byte)(id >> 8);
            act = (byte)(id & 0x00ff);
            this.flags = flags;
            this.bcc = bcc;
        }

        /// <summary>
        /// 计算bcc校验码
        /// </summary>
        /// <param name="buffer">数据</param>
        /// <param name="offset">偏移</param>
        /// <param name="length">数据长度</param>
        /// <returns></returns>
        public static byte CountBCC(byte[] buffer, int offset, int length)
        {
            byte value = 0x00;
            for (int i = offset; i < offset + length; i++)
            {
                value ^= buffer[i];
            }
            return value;
        }

        public override string ToString()
        {
            return $"msghead length:{length} cmd:{cmd} act:{act} flags:{flags} bcc:{bcc}";
        }
    }
}