using Microsoft.AspNetCore.Mvc;

namespace ChessWebApp.Controllers
{
    public class PlayController : Controller
    {
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
