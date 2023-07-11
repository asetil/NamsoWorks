using System;
using System.Linq;

namespace Aware.ECommerce.Util
{
    public static class Sungurlu
    {
        public static string Generate(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
            var result = new string(
                Enumerable.Repeat(chars, length)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }

        public static string ToPriceString(this decimal source)
        {
            return source.ToString("F", System.Globalization.CultureInfo.GetCultureInfo("tr-TR"));
        }
    }
}
