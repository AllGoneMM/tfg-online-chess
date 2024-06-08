using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Localization;

namespace ChessWebApp.Models.ViewModels
{
    public class LogInVM
    {
        [Required(ErrorMessage = "Username is required")]
        [Display(Prompt = "Username")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [Display(Prompt = "Password")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
