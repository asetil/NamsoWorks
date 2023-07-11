using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using Aware.Dependency;
using Aware.Util.Log;
using System.Net.Security;

namespace Aware.Util
{
    public static class WebRequester
    {
        public static T DoRequest<T>(string url, bool isPost, NameValueCollection parameters = null) where T : class
        {
            using (var client = new WebClient())
            {
                try
                {
                    string responseString;
                    if (isPost)
                    {
                        var response = client.UploadValues(url, parameters);
                        responseString = Encoding.Default.GetString(response);
                    }
                    else
                    {
                        responseString = client.DownloadString(url);
                    }
                    return responseString.DeSerialize<T>();
                }
                catch (Exception ex)
                {
                    var logger = WindsorBootstrapper.Resolve<ILogger>();
                    logger.Error("WebRequester > DoRequest - failed", ex);
                }
                return default(T);
            }
        }

        public static string DoRequest(string url, string userAgent = "", bool isPost = false, NameValueCollection parameters = null, NameValueCollection headers = null,string contentType="")
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using (var client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;

                if (string.IsNullOrEmpty(userAgent))
                {
                    userAgent = "Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)";
                }
                client.Headers.Add("user-agent", userAgent);

                if (string.IsNullOrEmpty(contentType))
                {
                    client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                }

                if (headers != null)
                {
                    client.Headers.Add(headers);
                }

                var responseString = string.Empty;
                if (isPost)
                {
                    client.UseDefaultCredentials = true;
                    client.Credentials = CredentialCache.DefaultCredentials;

                    var response = client.UploadValues(url, parameters);
                    responseString = Encoding.Default.GetString(response);
                }
                else
                {
                    responseString = client.DownloadString(url);
                }
                return responseString;
            }
        }

        public static string makePostRequest(string hostAddress, string data, NameValueCollection headers = null)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(hostAddress);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            request.Accept = "application/json";

            //request.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            if (headers != null)
            {
                request.Headers.Add(headers);
            }

            StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
            requestWriter.Write(data);
            requestWriter.Close();

            try
            {

                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                //ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(AllwaysGoodCertificate);

                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                //// Skip validation of SSL/TLS certificate
                //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };


                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                //  | SecurityProtocolType.Tls11
                //  | SecurityProtocolType.Tls12
                //  | SecurityProtocolType.Ssl3;

                WebResponse webResponse = request.GetResponse();
                Stream webStream = webResponse.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                string response = responseReader.ReadToEnd();
                responseReader.Close();

                return response;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private static bool AllwaysGoodCertificate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public static bool DownloadFile(string url, string filePath, string userAgent = "")
        {
            if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(filePath))
            {
                using (var client = new WebClient())
                {
                    if (string.IsNullOrEmpty(userAgent))
                    {
                        userAgent = "Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)";
                    }

                    client.Headers.Add("user-agent", userAgent);
                    client.DownloadFile(url, filePath);
                }
                return true;
            }
            return false;
        }

        public static string GetResponse(string url)
        {
            var request = WebRequest.Create(url) as HttpWebRequest;
            if (request != null)
            {
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    var reader = new StreamReader(response.GetResponseStream());
                    var responseString = reader.ReadToEnd(); //bunu kaldırabilirsin.
                    return responseString;
                }
            }
            return string.Empty;
        }

        private static string _filePath;
        public static void DownloadAsync(string url, string filePath)
        {
            _filePath = filePath;
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = "GET";
            webRequest.Timeout = 3000;
            webRequest.BeginGetResponse(new AsyncCallback(PlayResponeAsync), webRequest);
        }

        private static void PlayResponeAsync(IAsyncResult asyncResult)
        {
            long total = 0;
            int received = 0;
            HttpWebRequest webRequest = (HttpWebRequest)asyncResult.AsyncState;

            try
            {
                using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.EndGetResponse(asyncResult))
                {
                    byte[] buffer = new byte[1024];

                    FileStream fileStream = System.IO.File.OpenWrite(_filePath);
                    using (Stream input = webResponse.GetResponseStream())
                    {
                        //total = input.Length;

                        int size = input.Read(buffer, 0, buffer.Length);
                        while (size > 0)
                        {
                            fileStream.Write(buffer, 0, size);
                            received += size;

                            size = input.Read(buffer, 0, buffer.Length);
                        }
                    }

                    fileStream.Flush();
                    fileStream.Close();
                }
            }
            catch (Exception ex)
            {
            }
        }

        // private static dynamic GetJsonInternal(string queryString, params string[] args)
        // {
        //     var webAddress = string.Concat(this.mapServerUrl, "?", string.Format(queryString, args));
        //     using (WebClient webClient = new WebClient())
        //     {
        //         webClient.Encoding = Encoding.UTF8;
        //         var jsonData = webClient.DownloadString(webAddress);
        //         var jsonObject = jsonData.ToJson();

        //         return jsonObject;
        //     }
        // }
    }
}
