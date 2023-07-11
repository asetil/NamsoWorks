using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Aware.Dependency;
using Aware.File.Model;
using Aware.Util.Model;
using Aware.Util.Log;

namespace Aware.Util
{
    public static class Extensions
    {
        public static int Int(this string value, int defaultValue = 0)
        {
            try
            {
                if (!string.IsNullOrEmpty(value) && value.IsNumeric())
                {
                    return Convert.ToInt32(value);
                }
            }
            catch (Exception)
            {

            }
            return defaultValue;
        }

        public static bool IsNumeric(this string Value)
        {
            double retNum;
            return double.TryParse(Convert.ToString(Value), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
        }

        public static bool IsValidEmail(this string email, string regularExpression = "")
        {
            //var result = Regex.IsMatch(email, "^(?:[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+\\.)*[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+@(?:(?:(?:[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!\\.)){0,61}[a-zA-Z0-9]?\\.)+[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!$)){0,61}[a-zA-Z0-9]?)|(?:\\[(?:(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\.){3}(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\]))$", RegexOptions.IgnoreCase);

            if (string.IsNullOrEmpty(regularExpression)) { regularExpression = @"^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$"; }
            return Regex.IsMatch(email, regularExpression);
        }

        public static string[] Split(this string value, string splitString)
        {
            if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(splitString))
            {
                var result = new string[1];
                result[0] = splitString;
                return value.Split(result, StringSplitOptions.RemoveEmptyEntries);
            }
            return new[] { value };
        }

        public static string Short(this string value, int length, string leadingChars = "..")
        {
            if (!string.IsNullOrEmpty(value) && value.Length > length && length > 0)
            {
                return string.Format("{0}{1}", value.Trim().Substring(0, length), leadingChars);
            }
            return value;
        }

        public static string Capitalize(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return value.Substring(0, 1).ToUpperInvariant() + value.Substring(1);
            }
            return value;
        }

        public static string ToPrice(this decimal value, string currency, string format = "#,##0.00")
        {
            return string.Format("{0} {1}", value.ToString(format), currency);
        }

        public static string ToPrice(this decimal value)
        {
            return value.ToPrice(Common.Currency);
        }

        public static string DecString(this decimal value, string format = "#,##0.00")
        {
            return value.ToString(format);
        }

        //TODO# : 58300 => 58.300.00
        public static string DecToStr(this decimal value, string format = "N2")
        {
            return value.ToString(format).Replace(".", "").Replace(",", ".");
        }

        public static decimal Dec(this string value)
        {
            try
            {
                decimal result;
                decimal.TryParse(value.Replace(".", ","), out result);
                return result;
            }
            catch (Exception)
            {
            }
            return 0;
        }

        public static string Formatted(this decimal value, string format = "0.00")
        {
            return value.ToString(format).Replace(",", ".");
        }

        public static string[] Split(this decimal value, char splitChar = '.')
        {
            return value.Formatted().Split(splitChar);
        }

        public static string ToSeoUrl(this string value, bool deep = false)
        {
            var result = value;
            if (!string.IsNullOrEmpty(result))
            {
                result = result.ToLowerInvariant().Trim();
                if (deep)
                {
                    result = result.Replace(".net", "dot-net").Replace("c#", "csharp");
                }

                result = result.Replace(" - ", "-").Replace("ş", "s").Replace("ı", "i").Replace("İ", "i")
                    .Replace("ğ", "g").Replace("ü", "u").Replace("ç", "c").Replace("ö", "o")
                    .Replace("%", "").Replace("&", "ve").Replace("?", "")
                    .Replace(",", "").Replace("*", "").Replace("/", "").Replace(".", "").Replace(" ", "-");
            }
            return result;
        }

        public static string ToSqlLike(this string value)
        {
            if (string.IsNullOrEmpty(value)) { return string.Empty; }
            value = value.Replace("ş", "_").Replace("Ş", "_");
            value = value.Replace("ı", "_").Replace("İ", "_");
            value = value.Replace("ğ", "_").Replace("Ğ", "_");
            value = value.Replace("ü", "_").Replace("Ü", "_");
            value = value.Replace("ç", "_").Replace("Ç", "_");
            value = value.Replace("ö", "_").Replace("Ö", "_");
            return value;
        }

        public static string RemoveInjection(this string value)
        {
            if (string.IsNullOrEmpty(value)) { return string.Empty; }
            value = value.ToLowerInvariant();
            value = value.Replace("'", "");
            value = value.Replace("--", "");
            value = value.Replace(";", "");
            value = value.Replace("(", "");
            value = value.Replace(")", "");
            value = value.Replace("waitfor", "");
            value = value.Replace("delay", "");
            value = value.Replace("=", "");
            value = value.Replace("&gt;", "");
            value = value.Replace("&lt;", "");
            value = value.Replace("char ", "");
            value = value.Replace("delete from", "");
            value = value.Replace("insert ", "");
            value = value.Replace("update ", "");
            value = value.Replace("truncate ", "");
            value = value.Replace("script ", "");
            value = value.Replace("*", "");
            return value;
        }

        public static string ToSelected(this string value)
        {
            value = string.IsNullOrEmpty(value) ? string.Empty : value.Trim(',').S();
            return value;
        }

        public static List<Item> Add(this List<Item> list, int id, string value, string title = "")
        {
            if (list != null)
            {
                list.Add(new Item(id, title, value));
            }
            return list;
        }

        public static string S(this string value, string seperator = ",")
        {
            if (string.IsNullOrEmpty(value)) return value;
            return string.Format(",{0},", value);
        }

        public static string S(this int value, string seperator = ",")
        {
            return string.Format(",{0},", value);
        }

        public static string ToStr(this Enum value)
        {
            return (Convert.ToInt32(value)).ToString();
        }

        public static bool Contain(this string value, string subString)
        {
            return value.ToLowerInvariant().Contains(subString.ToLowerInvariant());
        }

        /// <summary>
        /// Combine two lambda expresions. Expresions params must be same
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left">left: f=>f...</param>
        /// <param name="right">right : f=>f...</param>
        /// <param name="filterType">or, and</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Combine<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right, Func<Expression, Expression, Expression> merge) where T : class
        {
            if (left != null && right != null)
            {
                var map = left.Parameters.Select((f, i) => new { f, s = right.Parameters[i] }).ToDictionary(p => p.s, p => p.f);
                var rightBody = ParameterRebinder.ReplaceParameters(map, right.Body);
                return Expression.Lambda<Func<T, bool>>(merge(left.Body, rightBody), left.Parameters);
            }
            return left ?? right;
        }

        public static int FirstID(this IEnumerable<int> idList)
        {
            if (idList != null)
            {
                return idList.FirstOrDefault();
            }
            return 0;
        }

        //public static string OptimizeFilter(this string filterAsString, int max, int min)
        //{
        //    if (!string.IsNullOrEmpty(filterAsString))
        //    {
        //        var rangeList = GetFilterRanges(filterAsString, max);
        //        if (rangeList.Count == 1)
        //        {
        //            var range = rangeList.FirstOrDefault();
        //            if (range.Value >= -1)
        //            {
        //                return string.Format("[{0},{1}]", range.Key, range.Value);
        //            }
        //            return string.Format("[{0},{0}]", range.Key);
        //        }
        //    }
        //    return string.Format("[{0},{1}]", min, max);
        //}

        public static TY Value<TX, TY>(this Dictionary<TX, TY> dictionary, TX key) where TY : class
        {
            if (dictionary != null && dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }
            return default(TY);
        }

        public static string LogString<TX, TY>(this Dictionary<TX, TY> dictionary) where TY : class
        {
            var result = string.Empty;
            if (dictionary != null)
            {
                foreach (var key in dictionary.Keys)
                {
                    var value = dictionary[key];
                    result += string.Format("{0}:{1};", key, value);
                }
            }
            return result;
        }

        public static List<FileRelation> GetFiles(this string modelAsJson)
        {
            try
            {
                //modelJson must be like [{"ID":1,"Path":"A","Name":"B"},{"ID":2,"Path":"C","Name":null},{"ID":3,"Path":"V","Name":"S"},{"ID":4,"Path":"B","Name":"D"}]
                var result = modelAsJson.DeSerialize<IEnumerable<FileRelation>>();
                return (result ?? new List<FileRelation>()).OrderBy(p => p.SortOrder ?? "999999").ToList();
            }
            catch (Exception ex)
            {
                var logger = WindsorBootstrapper.Resolve<ILogger>();
                logger.Error(string.Format("Extensions > GetFiles - Fail with modelAsJson:{0}", modelAsJson), ex);
            }
            return new List<FileRelation>();
        }

        public static string HashWithSignature(this string hashString, string signature)
        {
            var binaryHash = new HMACMD5(Encoding.UTF8.GetBytes(signature))
                .ComputeHash(Encoding.UTF8.GetBytes(hashString));

            var hash = BitConverter.ToString(binaryHash)
                .Replace("-", string.Empty)
                    .ToLowerInvariant();

            return hash;
        }
    }
}
