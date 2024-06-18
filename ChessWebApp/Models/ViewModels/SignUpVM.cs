using System.ComponentModel.DataAnnotations;

namespace ChessWebApp.Models.ViewModels
{
    public class SignUpVm
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email")]
        [Display(Prompt = "Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [Display(Prompt = "Username")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [Display(Prompt = "Password")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [Display(Prompt = "Confirm password")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
