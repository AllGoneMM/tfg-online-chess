using ChessWebApp.Identity;
using ChessWebApp.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChessWebApp.Controllers
{
    public class AccountController(SignInManager<ChessUser> signInManager) : Controller
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

        [HttpPost]
        public async Task<IActionResult> LogIn(LogInVM logInVm)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(logInVm.Email, logInVm.Password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Inicio de sesión incorrecto.");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Inicio de sesión incorrecto.");
            }
            return View();
        }
        [HttpPost]
        public IActionResult SignUp(SignUpVM signUpVM)
        {
            return new OkResult();
        }
    }
}
