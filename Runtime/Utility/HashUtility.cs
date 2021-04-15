namespace Framework
{
    public static partial class Utility
    {
        public static class Hash
        {
            /// <summary>
            /// 将两个hash转换成一个
            /// </summary>
            /// <param name="h1">hash1</param>
            /// <param name="h2">hash2</param>
            /// <returns>hash</returns>
            public static int CombineHash(int h1, int h2)
            {
                return h1 ^ (int)(h2 + 0x9e3779b9 + (h1 << 6) + (h1 >> 2));
            }

            /// <summary>
            /// 将三个hash转换成一个
            /// </summary>
            /// <param name="h1">hash1</param>
            /// <param name="h2">hash2</param>
            /// <param name="h3">hash3</param>
            /// <returns>hash</returns>
            public static int CombineHash(int h1, int h2, int h3)
            {
                return CombineHash(CombineHash(h1, h2), h3);
            }

            /// <summary>
            /// 将4个hash转换成一个
            /// </summary>
            /// <param name="h1">hash1</param>
            /// <param name="h2">hash2</param>
            /// <param name="h3">hash3</param>
            /// <param name="h4">hash4</param>
            /// <returns>hash</returns>
            public static int CombineHash(int h1, int h2, int h3, int h4)
            {
                return CombineHash(CombineHash(h1, h2, h3), h4);
            }

            /// <summary>
            /// 将5个hash转换成搞一个
            /// </summary>
            /// <param name="h1">hash1</param>
            /// <param name="h2">hash2</param>
            /// <param name="h3">hash3</param>
            /// <param name="h4">hash4</param>
            /// <param name="h5">hash5</param>
            /// <returns>hash</returns>
            public static int CombineHash(int h1, int h2, int h3, int h4, int h5)
            {
                return CombineHash(CombineHash(h1, h2, h3, h4), h5);
            }

            /// <summary>
            /// 将6个hash转换成一个
            /// </summary>
            /// <param name="h1">hash1</param>
            /// <param name="h2">hash2</param>
            /// <param name="h3">hash3</param>
            /// <param name="h4">hash4</param>
            /// <param name="h5">hash5</param>
            /// <param name="h6">hash6</param>
            /// <returns>hash</returns>
            public static int CombineHash(int h1, int h2, int h3, int h4, int h5, int h6)
            {
                return CombineHash(CombineHash(h1, h2, h3, h4, h5), h6);
            }

            /// <summary>
            /// 将7个hash转换成一个
            /// </summary>
            /// <param name="h1">hash1</param>
            /// <param name="h2">hash2</param>
            /// <param name="h3">hash3</param>
            /// <param name="h4">hash4</param>
            /// <param name="h5">hash5</param>
            /// <param name="h6">hash6</param>
            /// <param name="h7">hash7</param>
            /// <returns>hash</returns>
            public static int CombineHash(int h1, int h2, int h3, int h4, int h5, int h6, int h7)
            {
                return CombineHash(CombineHash(h1, h2, h3, h4, h5, h6), h7);
            }

            /// <summary>
            /// 将一个hash数组转换成一个
            /// </summary>
            /// <param name="hashes">hash数组</param>
            /// <returns>hash</returns>
            public static int CombineHash(int[] hashes)
            {
                if (hashes == null || hashes.Length == 0)
                    return 0;

                var h = hashes[0];
                for (int i = 1; i < hashes.Length; ++i)
                {
                    h = CombineHash(h, hashes[i]);
                }

                return h;
            }
        }
    }
}