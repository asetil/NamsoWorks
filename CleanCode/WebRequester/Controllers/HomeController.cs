using System.Net;
using System.Web.Mvc;

namespace WebRequester.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            Process();
            return View();
        }

        public ActionResult Info()
        {
            return View();
        }

        private void Process()
        {
            var domainList = System.IO.File.ReadAllLines(Server.MapPath("\\res\\domainlist.txt"));
            foreach (var domain in domainList)
            {
                ProcessRequest(domain);
            }
        }

        private string ProcessRequest(string url)
        {
            using (var client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                var userAgent = "Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)";
                client.Headers.Add("user-agent", userAgent);

                var responseString = client.DownloadString(url);
                return responseString;
            }
        }
    }
}