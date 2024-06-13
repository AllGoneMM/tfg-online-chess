using Microsoft.AspNetCore.Mvc;

namespace ChessWebApp.Controllers
{
    public class PlayController : Controller
    {
        [HttpGet("/test")]
        public IActionResult Test()
        {
            return View();
        }
        [HttpGet("/online")]
        public IActionResult Online()
        {
            return View();
        }
        [HttpGet("/offline")]
        public IActionResult Offline()
        {
            return View();
        }
    }
}
