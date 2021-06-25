using System;
using System.IO;
using System.IO.Compression;

namespace Framework.Service.Network
{
    public class DefaultNetworkCompressHelper : INetworkCompressHelper
    {
        public (byte[] bytes, string error) Compress(byte[] bytes)
        {
            try
            {
                using(var ms = new MemoryStream())
                {
                    using(var zip = new GZipStream(ms, CompressionMode.Compress, true))
                    {
                        zip.Write(bytes, 0, bytes.Length);
                        byte[] buffer = new byte[ms.Length];
                        ms.Position = 0;
                        var l = ms.Read(buffer, 0, buffer.Length);
                        return (buffer, null);
                    }
                }
            }
            catch (Exception e)
            {
                return (null, e.Message);
            }
        }

        byte[] decompressBuffer = new byte[0x1000];
        public (byte[] bytes, string error) Decompress(byte[] bytes)
        {
            try
            {
                using(var ms = new MemoryStream(bytes))
                {
                    using(var zip = new GZipStream(ms, CompressionMode.Decompress, true))
                    {
                        using(var msreader = new MemoryStream())
                        {
                            while (true)
                            {
                                int reader = zip.Read(decompressBuffer, 0, decompressBuffer.Length);
                                
                                if (reader <= 0)
                                {
                                    break;
                                }

                                msreader.Write(decompressBuffer, 0, reader);
                            }

                            msreader.Position = 0;
                            Array.Clear(decompressBuffer, 0, decompressBuffer.Length);
                            return (msreader.ToArray(), null);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return (null, e.Message);
            }
        }
    }
}