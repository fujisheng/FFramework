using System.Collections.Generic;
using UnityEngine;

namespace Framework.Utility
{
    public static class MathfUtility
    {

        /// <summary>
        /// 返回字典概率 {10, 50},{20, 50} 返回 10 或者 20 的几率都是50%
        /// </summary>
        /// <returns>The probability.</returns>
        /// <param name="comRandoms">COM randoms.</param>
        public static int GetProbability(Dictionary<int, int> comRandoms)
        {
            int range = Random.Range(0, 100);
            int offset = 0;

            foreach(var key in comRandoms.Keys)
            {
                int value = comRandoms[key];

                if (range >= 0 + offset && range < value + offset)
                {
                    return key;
                }
                offset += value;
            }

            return 0;
        }

        /// <summary>
        /// 将hex字符串转换成Color
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static Color HexToColor(string hex)
        {
            if (string.IsNullOrEmpty(hex))
            {
                return Color.black;
            }
            byte br = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte bg = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte bb = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            byte cc = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            float r = br / 255f;
            float g = bg / 255f;
            float b = bb / 255f;
            float a = cc / 255f;
            return new Color(r, g, b, a);
        }

        /// <summary>
        /// 将Color转换成hex字符串
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string ColorToHex(Color color)
        {
            int r = Mathf.RoundToInt(color.r * 255.0f);
            int g = Mathf.RoundToInt(color.g * 255.0f);
            int b = Mathf.RoundToInt(color.b * 255.0f);
            int a = Mathf.RoundToInt(color.a * 255.0f);
            string hex = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", r, g, b, a);
            return hex;
        }
    }
}