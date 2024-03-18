using Microsoft.AspNetCore.Mvc;
using MoscowWeather.Models;
using System.Diagnostics;

namespace MoscowWeather.Controllers
{
    public class HomeController : Controller
    {
        private WeatherContext db;

        public HomeController(WeatherContext context)
        {
            db = context;
        }
        public IActionResult Index()
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