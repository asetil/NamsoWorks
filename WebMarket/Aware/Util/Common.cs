using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Aware.Dependency;
using Aware.Util.Enums;
using System.Collections.Generic;

namespace Aware.Util
{
    [Serializable]
    public static class Common
    {
        /// <summary>
        ///   Get Object Value Throughout Reflection
        /// </summary>
        /// <param name="ErrorFlag">if false exception returns empty string</param>
        public static string GetValue(object obj, string name, bool ErrorFlag = true)
        {
            try
            {
                var nameValues = name.Split('.');
                object returnValue = "";
                foreach (var nameValue in nameValues)
                {
                    var propertyInfo = obj.GetType().GetProperty(nameValue);
                    if (propertyInfo != null)
                        returnValue = propertyInfo.GetValue(obj, null);
                    obj = returnValue;
                }
                return returnValue.ToString();
            }
            catch (Exception ex)
            {
                if (ErrorFlag) { throw new Exception(ex.Message + "\n" + "( Error while reading value: \"" + name + "\" )"); }
                return string.Empty;
            }
        }

        public static string GetDate(DateTime date, string seperator = " ", bool numeric = false)
        {
            var day = date.Day.ToString();
            var year = date.Year.ToString();
            var month = date.Month.ToString();

            if (!numeric)
            {
                if (month == "1") { month = "Ocak"; }
                else if (month == "2") { month = "Şubat"; }
                else if (month == "3") { month = "Mart"; }
                else if (month == "4") { month = "Nisan"; }
                else if (month == "5") { month = "Mayıs"; }
                else if (month == "6") { month = "Haziran"; }
                else if (month == "7") { month = "Temmuz"; }
                else if (month == "8") { month = "Ağustos"; }
                else if (month == "9") { month = "Eylül"; }
                else if (month == "10") { month = "Ekim"; }
                else if (month == "11") { month = "Kasım"; }
                else if (month == "12") { month = "Aralık"; }
            }
            return string.Format("{0}{3}{1}{3}{2}", day, month, year, seperator);
        }

        public static string Serialize(object input)
        {
            var webHelper = WindsorBootstrapper.Resolve<IWebHelper>();
            return webHelper.Serialize(input);
        }

        public static T DeSerialize<T>(this string input)
        {
            var webHelper = WindsorBootstrapper.Resolve<IWebHelper>();
            return webHelper.Deserialize<T>(input);
        }

        public static string HtmlStrip(string content, StripOptions options)
        {
            string result = string.Empty;
            var stripRegex = new Regex("<[^>]*>", RegexOptions.Compiled);

            if (!string.IsNullOrEmpty(content))
            {
                switch (options)
                {
                    case StripOptions.Default:
                        result = stripRegex.Replace(content, string.Empty);
                        break;
                    case StripOptions.WithoutLineBreaks:
                        content = content.Replace("<br>", Environment.NewLine).Replace("<br/>", Environment.NewLine).Replace("<br />", Environment.NewLine);
                        result = stripRegex.Replace(content, string.Empty);
                        break;
                    case StripOptions.WithoutLineBreaksAndDecode:
                        content = content.Replace("<br>", Environment.NewLine).Replace("<br/>", Environment.NewLine).Replace("<br />", Environment.NewLine);
                        result = HttpUtility.HtmlDecode(stripRegex.Replace(content, string.Empty));
                        break;
                }
            }

            return result;
        }

        public static string GetDayName(int index)
        {
            string Day = string.Empty;
            switch (index)
            {
                case 1: Day = "Pazartesi"; break;
                case 2: Day = "Salı"; break;
                case 3: Day = "Çarşamba"; break;
                case 4: Day = "Perşembe"; break;
                case 5: Day = "Cuma"; break;
                case 6: Day = "Cumartesi"; break;
                case 7: Day = "Pazar"; break;
            }
            if (index == (int)DateTime.Now.DayOfWeek) { Day += "*"; }
            return Day;
        }

        public static string GetRandomDigitCode(int length)
        {
            var random = new Random();
            string str = string.Empty;
            for (int i = 0; i < length; i++)
                str = String.Concat(str, random.Next(10).ToString());
            return str;
        }

        public static string Currency
        {
            get { return "TL"; }
        }

        public static string GetResourceTextFile(string filename)
        {
            string result = string.Empty;
            using (Stream stream = typeof(Common).Assembly.GetManifestResourceStream(filename))
            {
                if (stream != null)
                {
                    using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
                    {
                        result = streamReader.ReadToEnd();
                    }
                }
            }
            return result;
        }

        public static int GetOrderID(string idInfo)
        {
            var length = ECommerce.Util.Constants.ORDER_DATE_FORMAT.Length;
            if (!string.IsNullOrEmpty(idInfo) && idInfo.Length > length)
            {
                var result = idInfo.Substring(length).Int();
                return result;
            }
            return 0;
        }

        public static string AggregateDemo(List<string> collection)
        {
            string separator = ";";
            string result = collection.Aggregate((accu, item) => accu += string.Concat(separator, item));

            return result;
        }
    }
}
