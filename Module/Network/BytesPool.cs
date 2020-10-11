namespace Framework.Module.Network
{
    internal class BytesPool : ObjectPool<byte[]>
    {
        int MAX_READ = 2048 * 1024;
        public override int Size => 10;
        public BytesPool(int maxRead)
        {
            this.MAX_READ = maxRead;
        }
        protected override byte[] New()
        {
            return new byte[MAX_READ];
        }
    }
}
