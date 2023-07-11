using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Cinescope.Models;
using Cinescope.Manager;
using Aware.Util.Enum;

namespace Cinescope.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IFilmManager _filmManager;
        public HomeController(IFilmManager filmManager)
        {
            _filmManager = filmManager;
        }

        public IActionResult Index()
        {
            var filmSearchResult = _filmManager.GetFilms();
            return View(filmSearchResult);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
