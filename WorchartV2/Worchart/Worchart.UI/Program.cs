using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using RestSharp;

namespace Worchart.UI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var url = "http://82.114.81.185/Ziraat_QRCodeService/api/V1/identity/login";
            //var data = "UserId=test.violeta@ibank&Password=Password00&Step=1";
            ////var values = new System.Collections.Specialized.NameValueCollection();
            ////values.Add("UserId", "test.violeta@ibank");
            ////values.Add("Password", "Password00");
            ////values.Add("Step", "1");

            //var client2 = new WebClient();
            //client2.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
            //client2.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            //client2.Headers.Add(HttpRequestHeader.Accept, "application/json, text/json, text/x-json, text/javascript, application/xml, text/xml");
            //client2.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
            //client2.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            //var aa = client2.UploadString(url, data);

            //////var aa=client.UploadData("http://82.114.81.185/Ziraat_QRCodeService/api/V1/identity/login", data);
            //////var aa=client.UploadValues(url, values);

            ////byte[] byteArray = Encoding.ASCII.GetBytes(data);
            ////var aa=client.UploadData(url, byteArray);

            ////Dictionary<string, object> postParameters = new Dictionary<string, object>();
            ////postParameters.Add("UserId", "test.violeta@ibank");
            ////postParameters.Add("Password", "Password00");
            ////postParameters.Add("Step", 1);
            ////var bb = MultipartFormDataPost(url, "", postParameters);

            //var client = new RestSharp.RestClient(url);
            //var request = new RestRequest(Method.POST);
            //request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            //request.AddParameter("UserId", "test.violeta@ibank");
            //request.AddParameter("Password", "Password00");
            //request.AddParameter("Step", "1");

            //IRestResponse response = client.Execute(request);

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
        
    }
}
