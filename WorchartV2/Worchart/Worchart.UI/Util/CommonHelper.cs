using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Worchart.UI.Util
{
    public static class CommonHelper
    {
        public static T Deserialize<T>(this string json) where T : class
        {
            try
            {
                if (!string.IsNullOrEmpty(json))
                {
                    var textReader = new StringReader(json);
                    var jsonReader = new JsonTextReader(textReader);
                    return new JsonSerializer().Deserialize<T>(jsonReader);
                }
            }
            catch (Exception)
            {

            }
            return default(T);
        }

        public static string Serialize(this object data)
        {
            try
            {
                if (data != null)
                {
                    var sb = new StringBuilder();
                    var textWriter = new StringWriter(sb);
                    new JsonSerializer().Serialize(textWriter, data);
                    return sb.ToString();
                }
            }
            catch (Exception)
            {

            }
            return string.Empty;
        }
    }
}
