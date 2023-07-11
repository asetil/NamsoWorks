using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ArzTalep.Web.Models;
using Aware.Mail;

namespace ArzTalep.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMailManager _mailManager;
        public HomeController(IMailManager mailManager)
        {
            _mailManager = mailManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Campaign()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult MailTest()
        {
            _mailManager.Send("sj0ex3gvq4@the23app.com", "Deneme", "Deneme 1 2 ,3");
            return View("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
