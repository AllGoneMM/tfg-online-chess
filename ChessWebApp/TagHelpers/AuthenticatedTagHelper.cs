using ChessWebApp.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Http;

namespace ChessWebApp.TagHelpers
{
    [HtmlTargetElement(Attributes = "asp-authenticated")]
    public class AuthenticatedTagHelper : TagHelper
    {
        private readonly SignInManager<ChessUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticatedTagHelper(SignInManager<ChessUser> signInManager, IHttpContextAccessor httpContextAccessor)
        {
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
        }

        [HtmlAttributeName("asp-authenticated")]
        public bool? IsAuthenticated { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            bool isLoggedIn = _signInManager.IsSignedIn(_httpContextAccessor.HttpContext.User);

            // Logic based on a single IsAuthenticated attribute
            if (IsAuthenticated.HasValue)
            {
                if ((IsAuthenticated.Value && !isLoggedIn) || (!IsAuthenticated.Value && isLoggedIn))
                {
                    output.SuppressOutput();
                    return;
                }
            }
        }
    }
}