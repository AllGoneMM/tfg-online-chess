using ChessWebApp.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ChessWebApp.TagHelpers
{
    [HtmlTargetElement(Attributes = "asp-signed-in")]
    public class LoggedInTagHelper(SignInManager<ChessUser> signInManager, IHttpContextAccessor httpContextAccessor) : TagHelper
    {
        public bool IsLoggedIn { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (IsLoggedIn == null)
            {
                IsLoggedIn = signInManager.IsSignedIn(httpContextAccessor.HttpContext.User);
            }

            if (!IsLoggedIn)
            {
                output.SuppressOutput(); 
            }
        }
    }
}
