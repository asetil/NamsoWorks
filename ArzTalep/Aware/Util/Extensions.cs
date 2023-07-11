using Aware.File.Model;
using Aware.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Worchart.BL.Search;

namespace Aware.Util
{
    public static class Extensions
    {
        #region StringExtensions

        public static bool ValidEmail(this string email, string regularExpression = "")
        {
            if (email.Valid())
            {
                //var result = Regex.IsMatch(email, "^(?:[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+\\.)*[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+@(?:(?:(?:[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!\\.)){0,61}[a-zA-Z0-9]?\\.)+[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!$)){0,61}[a-zA-Z0-9]?)|(?:\\[(?:(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\.){3}(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\]))$", RegexOptions.IgnoreCase);

                if (string.IsNullOrEmpty(regularExpression)) { regularExpression = @"^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$"; }
                return Regex.IsMatch(email, regularExpression);
            }
            return false;
        }

        public static bool Valid(this string value)
        {
            return value != null && !string.IsNullOrEmpty(value.Trim());
        }

        public static int ToInt(this string value, int defaultValue = 0)
        {
            if (value.Valid())
            {
                return Convert.ToInt32(value);
            }
            return defaultValue;
        }

        public static string FormatWith(this string format, params object[] args)
        {
            if (format.Valid())
            {
                return string.Format(format, args);
            }
            return format;
        }

        #endregion

        /// <summary>
        /// Combine two lambda expresions. Expresions params must be same
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left">left: f=>f...</param>
        /// <param name="right">right : f=>f...</param>
        /// <param name="filterType">or, and</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Combine<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right, Func<Expression, Expression, Expression> merge) where T : IEntity
        {
            if (left != null && right != null)
            {
                var map = left.Parameters.Select((f, i) => new { f, s = right.Parameters[i] }).ToDictionary(p => p.s, p => p.f);
                var rightBody = ParameterRebinder.ReplaceParameters(map, right.Body);
                return Expression.Lambda<Func<T, bool>>(merge(left.Body, rightBody), left.Parameters);
            }
            return left ?? right;
        }

        #region Configuration

        public static string GetValue(this IConfiguration configuration, string key)
        {
            try
            {
                if (key.Valid())
                {
                    var value = configuration[key];
                    return value;
                }
            }
            catch (Exception ex)
            {

            }
            return string.Empty;
        }

        public static int GetInt(this IConfiguration configuration, string key)
        {
            try
            {
                var value = GetValue(configuration, key);
                return ToInt(value);
            }
            catch (Exception ex)
            {

            }
            return default(int);
        }

        public static T GetEnum<T>(this IConfiguration configuration, string key) where T : struct, IConvertible
        {
            try
            {
                var value = GetInt(configuration, key);
                return (T)(object)value;
            }
            catch (Exception ex)
            {

            }
            return default(T);
        }

        #endregion

        //public static List<FileRelation> GetFiles(this string modelAsJson)
        //{
        //    try
        //    {
        //        //modelJson must be like [{"ID":1,"Path":"A","Name":"B"},{"ID":2,"Path":"C","Name":null},{"ID":3,"Path":"V","Name":"S"},{"ID":4,"Path":"B","Name":"D"}]
        //        var result = modelAsJson.DeSerialize<IEnumerable<FileRelation>>();
        //        return (result ?? new List<FileRelation>()).OrderBy(p => p.SortOrder ?? "999999").ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        var logger = WindsorBootstrapper.Resolve<ILogger>();
        //        logger.Error(string.Format("Extensions > GetFiles - Fail with modelAsJson:{0}", modelAsJson), ex);
        //    }
        //    return new List<FileRelation>();
        //}

        public static dynamic ToDynamic(this object value)
        {
            IDictionary<string, object> expando = new ExpandoObject();

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(value.GetType()))
            {
                expando.Add(property.Name, property.GetValue(value));
            }

            return expando as ExpandoObject;
        }
    }
}
