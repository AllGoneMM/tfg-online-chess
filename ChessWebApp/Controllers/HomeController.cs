using ChessWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ChessWebApp.Controllers
{
    public class HomeController() : Controller
    {
        [HttpGet("/")]
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
