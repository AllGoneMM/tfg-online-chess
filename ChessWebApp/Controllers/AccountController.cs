using Microsoft.AspNetCore.Mvc;

namespace ChessWebApp.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult LogIn()
        {
            return View();
        }
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }
    }
}
