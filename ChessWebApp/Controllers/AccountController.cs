using ChessWebApp.Identity;
using ChessWebApp.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ChessWebApp.Controllers
{
    public class AccountController(SignInManager<ChessUser> signInManager, IStringLocalizer<AccountController> localizer, UserManager<ChessUser> userManager) : Controller
    {
        [HttpGet]
        public IActionResult LogIn()
        {
            if (User.Identity != null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            return View();
        }
        [HttpGet]
        public IActionResult SignUp()
        {
            if (User.Identity != null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogIn(LogInVM logInVm)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(logInVm.Username, logInVm.Password, logInVm.RememberMe, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("ErrorMessage", localizer["Log in attempt failed"]);
                }
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpVM signUpVM)
        {
            if (ModelState.IsValid)
            {
                ChessUser newUser = new ChessUser { UserName = signUpVM.Username, Email = signUpVM.Email };
                var result = await userManager.CreateAsync(newUser, signUpVM.Password);

                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(newUser, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("ErrorMessage", localizer["There was an error creating the account"]);
                }
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            if (signInManager.IsSignedIn(User))
            {
                await signInManager.SignOutAsync();
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
