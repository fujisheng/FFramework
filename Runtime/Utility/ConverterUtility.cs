using System;
using System.Text;

namespace Framework
{
    public static partial class Utility
    {
        public static class Converter
        {
            private const float InchesToCentimeters = 2.54f; // 1 inch = 2.54 cm
            private const float CentimetersToInches = 1f / InchesToCentimeters; // 1 cm = 0.3937 inches

            /// <summary>
            /// 获取数据在此计算机结构中存储时的字节顺序。
            /// </summary>
            public static bool IsLittleEndian
            {
                get
                {
                    return BitConverter.IsLittleEndian;
                }
            }

            /// <summary>
            /// 获取或设置屏幕每英寸点数。
            /// </summary>
            public static float ScreenDpi
            {
                get;
                set;
            }

            /// <summary>
            /// 将像素转换为厘米。
            /// </summary>
            /// <param name="pixels">像素。</param>
            /// <returns>厘米。</returns>
            public static float GetCentimetersFromPixels(float pixels)
            {
                Assert.IfTrue(ScreenDpi <= 0, new Exception("You must set screen DPI first."));

                return InchesToCentimeters * pixels / ScreenDpi;
            }

            /// <summary>
            /// 将厘米转换为像素。
            /// </summary>
            /// <param name="centimeters">厘米。</param>
            /// <returns>像素。</returns>
            public static float GetPixelsFromCentimeters(float centimeters)
            {
                Assert.IfTrue(ScreenDpi <= 0, new Exception("You must set screen DPI first."));

                return CentimetersToInches * centimeters * ScreenDpi;
            }

            /// <summary>
            /// 将像素转换为英寸。
            /// </summary>
            /// <param name="pixels">像素。</param>
            /// <returns>英寸。</returns>
            public static float GetInchesFromPixels(float pixels)
            {
                Assert.IfTrue(ScreenDpi <= 0, new Exception("You must set screen DPI first."));

                return pixels / ScreenDpi;
            }

            /// <summary>
            /// 将英寸转换为像素。
            /// </summary>
            /// <param name="inches">英寸。</param>
            /// <returns>像素。</returns>
            public static float GetPixelsFromInches(float inches)
            {
                Assert.IfTrue(ScreenDpi <= 0, new Exception("You must set screen DPI first."));

                return inches * ScreenDpi;
            }

            /// <summary>
            /// 以字节数组的形式获取指定的布尔值。
            /// </summary>
            /// <param name="value">要转换的布尔值。</param>
            /// <returns>用于存放结果的字节数组。</returns>
            public static byte[] GetBytes(bool value)
            {
                byte[] buffer = new byte[1];
                GetBytes(value, buffer, 0);
                return buffer;
            }

            /// <summary>
            /// 以字节数组的形式获取指定的布尔值。
            /// </summary>
            /// <param name="value">要转换的布尔值。</param>
            /// <param name="buffer">用于存放结果的字节数组。</param>
            public static void GetBytes(bool value, byte[] buffer)
            {
                GetBytes(value, buffer, 0);
            }

            /// <summary>
            /// 以字节数组的形式获取指定的布尔值。
            /// </summary>
            /// <param name="value">要转换的布尔值。</param>
            /// <param name="buffer">用于存放结果的字节数组。</param>
            /// <param name="startIndex">buffer 内的起始位置。</param>
            public static void GetBytes(bool value, byte[] buffer, int startIndex)
            {
                Assert.IfNull(buffer, new Exception("Buffer is invalid."));
                Assert.IfTrue(startIndex < 0 || startIndex + 1 > buffer.Length, new Exception("Start index is invalid."));

                buffer[startIndex] = value ? (byte)1 : (byte)0;
            }

            /// <summary>
            /// 返回由字节数组中首字节转换来的布尔值。
            /// </summary>
            /// <param name="value">字节数组。</param>
            /// <returns>如果 value 中的首字节非零，则为 true，否则为 false。</returns>
            public static bool GetBoolean(byte[] value)
            {
                return BitConverter.ToBoolean(value, 0);
            }

            /// <summary>
            /// 返回由字节数组中指定位置的一个字节转换来的布尔值。
            /// </summary>
            /// <param name="value">字节数组。</param>
            /// <param name="startIndex">value 内的起始位置。</param>
            /// <returns>如果 value 中指定位置的字节非零，则为 true，否则为 false。</returns>
            public static bool GetBoolean(byte[] value, int startIndex)
            {
                return BitConverter.ToBoolean(value, startIndex);
            }

            /// <summary>
            /// 以字节数组的形式获取指定的 Unicode 字符值。
            /// </summary>
            /// <param name="value">要转换的字符。</param>
            /// <returns>用于存放结果的字节数组。</returns>
            public static byte[] GetBytes(char value)
            {
                byte[] buffer = new byte[2];
                GetBytes((short)value, buffer, 0);
                return buffer;
            }

            /// <summary>
            /// 以字节数组的形式获取指定的 Unicode 字符值。
            /// </summary>
            /// <param name="value">要转换的字符。</param>
            /// <param name="buffer">用于存放结果的字节数组。</param>
            public static void GetBytes(char value, byte[] buffer)
            {
                GetBytes((short)value, buffer, 0);
            }

            /// <summary>
            /// 以字节数组的形式获取指定的 Unicode 字符值。
            /// </summary>
            /// <param name="value">要转换的字符。</param>
            /// <param name="buffer">用于存放结果的字节数组。</param>
            /// <param name="startIndex">buffer 内的起始位置。</param>
            public static void GetBytes(char value, byte[] buffer, int startIndex)
            {
                GetBytes((short)value, buffer, startIndex);
            }

            /// <summary>
            /// 返回由字节数组中前两个字节转换来的 Unicode 字符。
            /// </summary>
            /// <param name="value">字节数组。</param>
            /// <returns>由两个字节构成的字符。</returns>
            public static char GetChar(byte[] value)
            {
                return BitConverter.ToChar(value, 0);
            }

            /// <summary>
            /// 返回由字节数组中指定位置的两个字节转换来的 Unicode 字符。
            /// </summary>
            /// <param name="value">字节数组。</param>
            /// <param name="startIndex">value 内的起始位置。</param>
            /// <returns>由两个字节构成的字符。</returns>
            public static char GetChar(byte[] value, int startIndex)
            {
                return BitConverter.ToChar(value, startIndex);
            }

            /// <summary>
            /// 以字节数组的形式获取指定的 16 位有符号整数值。
            /// </summary>
            /// <param name="value">要转换的数字。</param>
            /// <returns>用于存放结果的字节数组。</returns>
            public static byte[] GetBytes(short value)
            {
                byte[] buffer = new byte[2];
                GetBytes(value, buffer, 0);
                return buffer;
            }

            /// <summary>
            /// 以字节数组的形式获取指定的 16 位有符号整数值。
            /// </summary>
            /// <param name="value">要转换的数字。</param>
            /// <param name="buffer">用于存放结果的字节数组。</param>
            public static void GetBytes(short value, byte[] buffer)
            {
                GetBytes(value, buffer, 0);
            }

            /// <summary>
            /// 以字节数组的形式获取指定的 16 位有符号整数值。
            /// </summary>
            /// <param name="value">要转换的数字。</param>
            /// <param name="buffer">用于存放结果的字节数组。</param>
            /// <param name="startIndex">buffer 内的起始位置。</param>
            public static unsafe void GetBytes(short value, byte[] buffer, int startIndex)
            {
                Assert.IfNull(buffer, new Exception("Buffer is invalid."));
                Assert.IfTrue(startIndex < 0 || startIndex + 2 > buffer.Length, new Exception("Start index is invalid."));

                fixed (byte* valueRef = buffer)
                {
                    *(short*)(valueRef + startIndex) = value;
                }
            }

            /// <summary>
            /// 返回由字节数组中前两个字节转换来的 16 位有符号整数。
            /// </summary>
            /// <param name="value">字节数组。</param>
            /// <returns>由两个字节构成的 16 位有符号整数。</returns>
            public static short GetInt16(byte[] value)
            {
                return BitConverter.ToInt16(value, 0);
            }

            /// <summary>
            /// 返回由字节数组中指定位置的两个字节转换来的 16 位有符号整数。
            /// </summary>
            /// <param name="value">字节数组。</param>
            /// <param name="startIndex">value 内的起始位置。</param>
            /// <returns>由两个字节构成的 16 位有符号整数。</returns>
            public static short GetInt16(byte[] value, int startIndex)
            {
                return BitConverter.ToInt16(value, startIndex);
            }

            /// <summary>
            /// 以字节数组的形式获取指定的 16 位无符号整数值。
            /// </summary>
            /// <param name="value">要转换的数字。</param>
            /// <returns>用于存放结果的字节数组。</returns>
            public static byte[] GetBytes(ushort value)
            {
                byte[] buffer = new byte[2];
                GetBytes((short)value, buffer, 0);
                return buffer;
            }

            /// <summary>
            /// 以字节数组的形式获取指定的 16 位无符号整数值。
            /// </summary>
            /// <param name="value">要转换的数字。</param>
            /// <param name="buffer">用于存放结果的字节数组。</param>
            public static void GetBytes(ushort value, byte[] buffer)
            {
                GetBytes((short)value, buffer, 0);
            }

            /// <summary>
            /// 以字节数组的形式获取指定的 16 位无符号整数值。
            /// </summary>
            /// <param name="value">要转换的数字。</param>
            /// <param name="buffer">用于存放结果的字节数组。</param>
            /// <param name="startIndex">buffer 内的起始位置。</param>
            public static void GetBytes(ushort value, byte[] buffer, int startIndex)
            {
                GetBytes((short)value, buffer, startIndex);
            }

            /// <summary>
            /// 返回由字节数组中前两个字节转换来的 16 位无符号整数。
            /// </summary>
            /// <param name="value">字节数组。</param>
            /// <returns>由两个字节构成的 16 位无符号整数。</returns>
            public static ushort GetUInt16(byte[] value)
            {
                return BitConverter.ToUInt16(value, 0);
            }

            /// <summary>
            /// 返回由字节数组中指定位置的两个字节转换来的 16 位无符号整数。
            /// </summary>
            /// <param name="value">字节数组。</param>
            /// <param name="startIndex">value 内的起始位置。</param>
            /// <returns>由两个字节构成的 16 位无符号整数。</returns>
            public static ushort GetUInt16(byte[] value, int startIndex)
            {
                return BitConverter.ToUInt16(value, startIndex);
            }

            /// <summary>
            /// 以字节数组的形式获取指定的 32 位有符号整数值。
            /// </summary>
            /// <param name="value">要转换的数字。</param>
            /// <returns>用于存放结果的字节数组。</returns>
            public static byte[] GetBytes(int value)
            {
                byte[] buffer = new byte[4];
                GetBytes(value, buffer, 0);
                return buffer;
            }

            /// <summary>
            /// 以字节数组的形式获取指定的 32 位有符号整数值。
            /// </summary>
            /// <param name="value">要转换的数字。</param>
            /// <param name="buffer">用于存放结果的字节数组。</param>
            public static void GetBytes(int value, byte[] buffer)
            {
                GetBytes(value, buffer, 0);
            }

            /// <summary>
            /// 以字节数组的形式获取指定的 32 位有符号整数值。
            /// </summary>
            /// <param name="value">要转换的数字。</param>
            /// <param name="buffer">用于存放结果的字节数组。</param>
            /// <param name="startIndex">buffer 内的起始位置。</param>
            public static unsafe void GetBytes(int value, byte[] buffer, int startIndex)
            {
                Assert.IfNull(buffer, new Exception("Buffer is invalid."));
                Assert.IfTrue(startIndex < 0 || startIndex + 4 > buffer.Length, new Exception("Start index is invalid."));

                fixed (byte* valueRef = buffer)
                {
                    *(int*)(valueRef + startIndex) = value;
                }
            }

            /// <summary>
            /// 返回由字节数组中前四个字节转换来的 32 位有符号整数。
            /// </summary>
            /// <param name="value">字节数组。</param>
            /// <returns>由四个字节构成的 32 位有符号整数。</returns>
            public static int GetInt32(byte[] value)
            {
                return BitConverter.ToInt32(value, 0);
            }

            /// <summary>
            /// 返回由字节数组中指定位置的四个字节转换来的 32 位有符号整数。
            /// </summary>
            /// <param name="value">字节数组。</param>
            /// <param name="startIndex">value 内的起始位置。</param>
            /// <returns>由四个字节构成的 32 位有符号整数。</returns>
            public static int GetInt32(byte[] value, int startIndex)
            {
                return BitConverter.ToInt32(value, startIndex);
            }

            /// <summary>
            /// 以字节数组的形式获取指定的 32 位无符号整数值。
            /// </summary>
            /// <param name="value">要转换的数字。</param>
            /// <returns>用于存放结果的字节数组。</returns>
            public static byte[] GetBytes(uint value)
            {
                byte[] buffer = new byte[4];
                GetBytes((int)value, buffer, 0);
                return buffer;
            }

            /// <summary>
            /// 以字节数组的形式获取指定的 32 位无符号整数值。
            /// </summary>
            /// <param name="value">要转换的数字。</param>
            /// <param name="buffer">用于存放结果的字节数组。</param>
            public static void GetBytes(uint value, byte[] buffer)
            {
                GetBytes((int)value, buffer, 0);
            }

            /// <summary>
            /// 以字节数组的形式获取指定的 32 位无符号整数值。
            /// </summary>
            /// <param name="value">要转换的数字。</param>
            /// <param name="buffer">用于存放结果的字节数组。</param>
            /// <param name="startIndex">buffer 内的起始位置。</param>
            public static void GetBytes(uint value, byte[] buffer, int startIndex)
            {
                GetBytes((int)value, buffer, startIndex);
            }

            /// <summary>
            /// 返回由字节数组中前四个字节转换来的 32 位无符号整数。
            /// </summary>
            /// <param name="value">字节数组。</param>
            /// <returns>由四个字节构成的 32 位无符号整数。</returns>
            public static uint GetUInt32(byte[] value)
            {
                return BitConverter.ToUInt32(value, 0);
            }

            /// <summary>
            /// 返回由字节数组中指定位置的四个字节转换来的 32 位无符号整数。
            /// </summary>
            /// <param name="value">字节数组。</param>
            /// <param name="startIndex">value 内的起始位置。</param>
            /// <returns>由四个字节构成的 32 位无符号整数。</returns>
            public static uint GetUInt32(byte[] value, int startIndex)
            {
                return BitConverter.ToUInt32(value, startIndex);
            }

            /// <summary>
            /// 以字节数组的形式获取指定的 64 位有符号整数值。
            /// </summary>
            /// <param name="value">要转换的数字。</param>
            /// <returns>用于存放结果的字节数组。</returns>
            public static byte[] GetBytes(long value)
            {
                byte[] buffer = new byte[8];
                GetBytes(value, buffer, 0);
                return buffer;
            }

            /// <summary>
            /// 以字节数组的形式获取指定的 64 位有符号整数值。
            /// </summary>
            /// <param name="value">要转换的数字。</param>
            /// <param name="buffer">用于存放结果的字节数组。</param>
            public static void GetBytes(long value, byte[] buffer)
            {
                GetBytes(value, buffer, 0);
            }

            /// <summary>
            /// 以字节数组的形式获取指定的 64 位有符号整数值。
            /// </summary>
            /// <param name="value">要转换的数字。</param>
            /// <param name="buffer">用于存放结果的字节数组。</param>
            /// <param name="startIndex">buffer 内的起始位置。</param>
            public static unsafe void GetBytes(long value, byte[] buffer, int startIndex)
            {
                Assert.IfNull(buffer, new Exception("Buffer is invalid."));
                Assert.IfTrue(startIndex < 0 || startIndex + 8 > buffer.Length, new Exception("Start index is invalid."));

                fixed (byte* valueRef = buffer)
                {
                    *(long*)(valueRef + startIndex) = value;
                }
            }

            /// <summary>
            /// 返回由字节数组中前八个字节转换来的 64 位有符号整数。
            /// </summary>
            /// <param name="value">字节数组。</param>
            /// <returns>由八个字节构成的 64 位有符号整数。</returns>
            public static long GetInt64(byte[] value)
            {
                return BitConverter.ToInt64(value, 0);
            }

            /// <summary>
            /// 返回由字节数组中指定位置的八个字节转换来的 64 位有符号整数。
            /// </summary>
            /// <param name="value">字节数组。</param>
            /// <param name="startIndex">value 内的起始位置。</param>
            /// <returns>由八个字节构成的 64 位有符号整数。</returns>
            public static long GetInt64(byte[] value, int startIndex)
            {
                return BitConverter.ToInt64(value, startIndex);
            }

            /// <summary>
            /// 以字节数组的形式获取指定的 64 位无符号整数值。
            /// </summary>
            /// <param name="value">要转换的数字。</param>
            /// <returns>用于存放结果的字节数组。</returns>
            public static byte[] GetBytes(ulong value)
            {
                byte[] buffer = new byte[8];
                GetBytes((long)value, buffer, 0);
                return buffer;
            }

            /// <summary>
            /// 以字节数组的形式获取指定的 64 位无符号整数值。
            /// </summary>
            /// <param name="value">要转换的数字。</param>
            /// <param name="buffer">用于存放结果的字节数组。</param>
            public static void GetBytes(ulong value, byte[] buffer)
            {
                GetBytes((long)value, buffer, 0);
            }

            /// <summary>
            /// 以字节数组的形式获取指定的 64 位无符号整数值。
            /// </summary>
            /// <param name="value">要转换的数字。</param>
            /// <param name="buffer">用于存放结果的字节数组。</param>
            /// <param name="startIndex">buffer 内的起始位置。</param>
            public static void GetBytes(ulong value, byte[] buffer, int startIndex)
            {
                GetBytes((long)value, buffer, startIndex);
            }

            /// <summary>
            /// 返回由字节数组中前八个字节转换来的 64 位无符号整数。
            /// </summary>
            /// <param name="value">字节数组。</param>
            /// <returns>由八个字节构成的 64 位无符号整数。</returns>
            public static ulong GetUInt64(byte[] value)
            {
                return BitConverter.ToUInt64(value, 0);
            }

            /// <summary>
            /// 返回由字节数组中指定位置的八个字节转换来的 64 位无符号整数。
            /// </summary>
            /// <param name="value">字节数组。</param>
            /// <param name="startIndex">value 内的起始位置。</param>
            /// <returns>由八个字节构成的 64 位无符号整数。</returns>
            public static ulong GetUInt64(byte[] value, int startIndex)
            {
                return BitConverter.ToUInt64(value, startIndex);
            }

            /// <summary>
            /// 以字节数组的形式获取指定的单精度浮点值。
            /// </summary>
            /// <param name="value">要转换的数字。</param>
            /// <returns>用于存放结果的字节数组。</returns>
            public static unsafe byte[] GetBytes(float value)
            {
                byte[] buffer = new byte[4];
                GetBytes(*(int*)&value, buffer, 0);
                return buffer;
            }

            /// <summary>
            /// 以字节数组的形式获取指定的单精度浮点值。
            /// </summary>
            /// <param name="value">要转换的数字。</param>
            /// <param name="buffer">用于存放结果的字节数组。</param>
            public static unsafe void GetBytes(float value, byte[] buffer)
            {
                GetBytes(*(int*)&value, buffer, 0);
            }

            /// <summary>
            /// 以字节数组的形式获取指定的单精度浮点值。
            /// </summary>
            /// <param name="value">要转换的数字。</param>
            /// <param name="buffer">用于存放结果的字节数组。</param>
            /// <param name="startIndex">buffer 内的起始位置。</param>
            public static unsafe void GetBytes(float value, byte[] buffer, int startIndex)
            {
                GetBytes(*(int*)&value, buffer, startIndex);
            }

            /// <summary>
            /// 返回由字节数组中前四个字节转换来的单精度浮点数。
            /// </summary>
            /// <param name="value">字节数组。</param>
            /// <returns>由四个字节构成的单精度浮点数。</returns>
            public static float GetSingle(byte[] value)
            {
                return BitConverter.ToSingle(value, 0);
            }

            /// <summary>
            /// 返回由字节数组中指定位置的四个字节转换来的单精度浮点数。
            /// </summary>
            /// <param name="value">字节数组。</param>
            /// <param name="startIndex">value 内的起始位置。</param>
            /// <returns>由四个字节构成的单精度浮点数。</returns>
            public static float GetSingle(byte[] value, int startIndex)
            {
                return BitConverter.ToSingle(value, startIndex);
            }

            /// <summary>
            /// 以字节数组的形式获取指定的双精度浮点值。
            /// </summary>
            /// <param name="value">要转换的数字。</param>
            /// <returns>用于存放结果的字节数组。</returns>
            public static unsafe byte[] GetBytes(double value)
            {
                byte[] buffer = new byte[8];
                GetBytes(*(long*)&value, buffer, 0);
                return buffer;
            }

            /// <summary>
            /// 以字节数组的形式获取指定的双精度浮点值。
            /// </summary>
            /// <param name="value">要转换的数字。</param>
            /// <param name="buffer">用于存放结果的字节数组。</param>
            public static unsafe void GetBytes(double value, byte[] buffer)
            {
                GetBytes(*(long*)&value, buffer, 0);
            }

            /// <summary>
            /// 以字节数组的形式获取指定的双精度浮点值。
            /// </summary>
            /// <param name="value">要转换的数字。</param>
            /// <param name="buffer">用于存放结果的字节数组。</param>
            /// <param name="startIndex">buffer 内的起始位置。</param>
            public static unsafe void GetBytes(double value, byte[] buffer, int startIndex)
            {
                GetBytes(*(long*)&value, buffer, startIndex);
            }

            /// <summary>
            /// 返回由字节数组中前八个字节转换来的双精度浮点数。
            /// </summary>
            /// <param name="value">字节数组。</param>
            /// <returns>由八个字节构成的双精度浮点数。</returns>
            public static double GetDouble(byte[] value)
            {
                return BitConverter.ToDouble(value, 0);
            }

            /// <summary>
            /// 返回由字节数组中指定位置的八个字节转换来的双精度浮点数。
            /// </summary>
            /// <param name="value">字节数组。</param>
            /// <param name="startIndex">value 内的起始位置。</param>
            /// <returns>由八个字节构成的双精度浮点数。</returns>
            public static double GetDouble(byte[] value, int startIndex)
            {
                return BitConverter.ToDouble(value, startIndex);
            }

            /// <summary>
            /// 以字节数组的形式获取 UTF-8 编码的字符串。
            /// </summary>
            /// <param name="value">要转换的字符串。</param>
            /// <returns>用于存放结果的字节数组。</returns>
            public static byte[] GetBytes(string value)
            {
                return GetBytes(value, Encoding.UTF8);
            }

            /// <summary>
            /// 以字节数组的形式获取 UTF-8 编码的字符串。
            /// </summary>
            /// <param name="value">要转换的字符串。</param>
            /// <param name="buffer">用于存放结果的字节数组。</param>
            /// <returns>buffer 内实际填充了多少字节。</returns>
            public static int GetBytes(string value, byte[] buffer)
            {
                return GetBytes(value, Encoding.UTF8, buffer, 0);
            }

            /// <summary>
            /// 以字节数组的形式获取 UTF-8 编码的字符串。
            /// </summary>
            /// <param name="value">要转换的字符串。</param>
            /// <param name="buffer">用于存放结果的字节数组。</param>
            /// <param name="startIndex">buffer 内的起始位置。</param>
            /// <returns>buffer 内实际填充了多少字节。</returns>
            public static int GetBytes(string value, byte[] buffer, int startIndex)
            {
                return GetBytes(value, Encoding.UTF8, buffer, startIndex);
            }

            /// <summary>
            /// 以字节数组的形式获取指定编码的字符串。
            /// </summary>
            /// <param name="value">要转换的字符串。</param>
            /// <param name="encoding">要使用的编码。</param>
            /// <returns>用于存放结果的字节数组。</returns>
            public static byte[] GetBytes(string value, Encoding encoding)
            {
                Assert.IfNull(value, new Exception("Value is invalid."));
                Assert.IfNull(encoding, new Exception("Encoding is invalid."));

                return encoding.GetBytes(value);
            }

            /// <summary>
            /// 以字节数组的形式获取指定编码的字符串。
            /// </summary>
            /// <param name="value">要转换的字符串。</param>
            /// <param name="encoding">要使用的编码。</param>
            /// <param name="buffer">用于存放结果的字节数组。</param>
            /// <returns>buffer 内实际填充了多少字节。</returns>
            public static int GetBytes(string value, Encoding encoding, byte[] buffer)
            {
                return GetBytes(value, encoding, buffer, 0);
            }

            /// <summary>
            /// 以字节数组的形式获取指定编码的字符串。
            /// </summary>
            /// <param name="value">要转换的字符串。</param>
            /// <param name="encoding">要使用的编码。</param>
            /// <param name="buffer">用于存放结果的字节数组。</param>
            /// <param name="startIndex">buffer 内的起始位置。</param>
            /// <returns>buffer 内实际填充了多少字节。</returns>
            public static int GetBytes(string value, Encoding encoding, byte[] buffer, int startIndex)
            {
                Assert.IfNull(value, new Exception("Value is invalid."));
                Assert.IfNull(encoding, new Exception("Encoding is invalid."));

                return encoding.GetBytes(value, 0, value.Length, buffer, startIndex);
            }

            /// <summary>
            /// 返回由字节数组使用 UTF-8 编码转换成的字符串。
            /// </summary>
            /// <param name="value">字节数组。</param>
            /// <returns>转换后的字符串。</returns>
            public static string GetString(byte[] value)
            {
                return GetString(value, Encoding.UTF8);
            }

            /// <summary>
            /// 返回由字节数组使用指定编码转换成的字符串。
            /// </summary>
            /// <param name="value">字节数组。</param>
            /// <param name="encoding">要使用的编码。</param>
            /// <returns>转换后的字符串。</returns>
            public static string GetString(byte[] value, Encoding encoding)
            {
                Assert.IfNull(value, new Exception("Value is invalid."));
                Assert.IfNull(encoding, new Exception("Encoding is invalid."));

                return encoding.GetString(value);
            }

            /// <summary>
            /// 返回由字节数组使用 UTF-8 编码转换成的字符串。
            /// </summary>
            /// <param name="value">字节数组。</param>
            /// <param name="startIndex">value 内的起始位置。</param>
            /// <param name="length">长度。</param>
            /// <returns>转换后的字符串。</returns>
            public static string GetString(byte[] value, int startIndex, int length)
            {
                return GetString(value, startIndex, length, Encoding.UTF8);
            }

            /// <summary>
            /// 返回由字节数组使用指定编码转换成的字符串。
            /// </summary>
            /// <param name="value">字节数组。</param>
            /// <param name="startIndex">value 内的起始位置。</param>
            /// <param name="length">长度。</param>
            /// <param name="encoding">要使用的编码。</param>
            /// <returns>转换后的字符串。</returns>
            public static string GetString(byte[] value, int startIndex, int length, Encoding encoding)
            {
                Assert.IfNull(value, new Exception("Value is invalid."));
                Assert.IfNull(encoding, new Exception("Encoding is invalid."));

                return encoding.GetString(value, startIndex, length);
            }

            /// <summary>
            /// 将4位比特转换成int32
            /// </summary>
            /// <param name="byte1">byte1</param>
            /// <param name="byte2">byte2</param>
            /// <param name="byte3">byte3</param>
            /// <param name="byte4">byte4</param>
            /// <param name="reverse">是否反转</param>
            /// <returns></returns>
            public static int GetInt32(byte byte1, byte byte2, byte byte3, byte byte4, bool reverse = false)
            {
                if (reverse)
                {
                    var result = byte4 & 0xff;
                    result |= byte3 << 8 & 0xff00;
                    result |= byte2 << 16 & 0xff0000;
                    result |= (int)(byte1 << 24 & 0xff000000);
                    return result;
                }
                else
                {
                    var result = byte1 & 0xff;
                    result |= byte2 << 8 & 0xff00;
                    result |= byte3 << 16 & 0xff0000;
                    result |= (int)(byte4 << 24 & 0xff000000);
                    return result;
                }
            }

            /// <summary>
            /// 将4位比特转换成uint32
            /// </summary>
            /// <param name="byte1">byte1</param>
            /// <param name="byte2">byte2</param>
            /// <param name="byte3">byte3</param>
            /// <param name="byte4">byte4</param>
            /// <param name="reverse">是否反转</param>
            /// <returns></returns>
            public static uint GetUInt32(byte byte1, byte byte2, byte byte3, byte byte4, bool reverse = false)
            {
                if (reverse)
                {
                    var result = byte4 & 0xff;
                    result |= (byte3 & 0xff) << 8;
                    result |= (byte2 & 0xff) << 16;
                    result |= (byte1 & 0xff) << 24;
                    result = (int)(result & 0xFFFFFFFFL);
                    return (uint)result;
                }
                else
                {
                    var result = byte1 & 0xff;
                    result |= (byte2 & 0xff) << 8;
                    result |= (byte3 & 0xff) << 16;
                    result |= (byte4 & 0xff) << 24;
                    result = (int)(result & 0xFFFFFFFFL);
                    return (uint)result;
                }
            }

            /// <summary>
            /// 获取UInt32 只能是四位bytes
            /// </summary>
            /// <param name="bytes">bytes</param>
            /// <param name="startIndex">开始索引</param>
            /// <param name="reverse">是否反转</param>
            /// <returns></returns>
            public static uint GetUInt32(byte[] bytes, int startIndex, bool reverse = false)
            {
                if(bytes.Length < startIndex + 4)
                {
                    return default;
                }
                return GetUInt32(bytes[startIndex], bytes[startIndex + 1], bytes[startIndex + 2], bytes[startIndex + 3], reverse);
            }
        }
    }
}
