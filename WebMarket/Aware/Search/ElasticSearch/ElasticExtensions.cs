using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using Nest;

namespace Aware.Search.ElasticSearch
{
    public static class ElasticExtensions
    {
        public static string Utf8String(this byte[] bytes)
        {
            return bytes == null ? null : Encoding.UTF8.GetString(bytes);
        }

        public static string ToElasticDate(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fff", CultureInfo.InvariantCulture);
        }

        public static string ToElasticKeyword(this string strIn)
        {
            strIn = strIn.ToLower();

            string[] olds = { "ğ", "ü", "ş", "ı", "ö", "ç" };
            string[] news = { "g", "u", "s", "i", "o", "c" };

            for (var i = 0; i < olds.Length; i++)
            {
                strIn = strIn.Replace(olds[i], news[i]);
            }
            var regex = new Regex("[^a-zA-Z0-9 ]", RegexOptions.Compiled);
            return regex.Replace(strIn, String.Empty).Trim();
        }

        public static string ToNestedReverse(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return value+"_nested";
            }
            return value;
        }

        public static string ToExactMatchString(this string strIn, bool stripHtml = false)
        {
            if (stripHtml) strIn = strIn.StripTagsCharArray();

            strIn = strIn.ToLower();

            string[] olds = { "ğ", "ü", "ş", "ı", "ö", "ç" };
            string[] news = { "g", "u", "s", "i", "o", "c" };

            for (var i = 0; i < olds.Length; i++)
            {
                strIn = strIn.Replace(olds[i], news[i]);
            }

            return new string(strIn.Where(char.IsLetterOrDigit).ToArray());
        }


        private static string StripTagsCharArray(this string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }

        public static string ToCamelCase(this string strIn)
        {
            if (!string.IsNullOrEmpty(strIn))
            {
                return strIn.Substring(0, 1).ToLower() + strIn.Substring(1);
            }
            return string.Empty;
        }
    }
}
