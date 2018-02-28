using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMMO.Util
{
    public static class NumberUtil
    {

        public static readonly Random random = new Random();
        public static readonly object randomLock = new object();

        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static bool IsNegative(this int val)
        {
            return val < 0;
        }

        public static float Range(this Random random, float minimum, float maximum)
        {
            return (float)random.NextDouble() * (maximum - minimum) + minimum;
        }

    }
}
