using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ChessWebApp.TagHelpers
{
    [HtmlTargetElement(Attributes = "asp-active-class")]
    public class ActiveRouteTagHelper : TagHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ActiveRouteTagHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        [HtmlAttributeName("asp-active-action-controller")]
        public string ControllerActionPairs { get; set; }

        [HtmlAttributeName("asp-active-class")]
        public string ActiveClass { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var routeData = _httpContextAccessor.HttpContext.GetRouteData();
            var currentController = routeData.Values["controller"]?.ToString();
            var currentAction = routeData.Values["action"]?.ToString();

            var pairs = ParseControllerActionPairs(ControllerActionPairs);

            if (pairs.Any(pair => string.Equals(pair.Controller, currentController, StringComparison.OrdinalIgnoreCase) &&
                                  string.Equals(pair.Action, currentAction, StringComparison.OrdinalIgnoreCase)))
            {
                var existingClasses = output.Attributes["class"]?.Value?.ToString();
                var classes = string.IsNullOrEmpty(existingClasses) ? ActiveClass : $"{existingClasses} {ActiveClass}";

                output.Attributes.SetAttribute("class", classes);
            }
        }

        private List<(string Controller, string Action)> ParseControllerActionPairs(string input)
        {
            var pairs = new List<(string Controller, string Action)>();
            var items = input?.Split(';', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

            foreach (var item in items)
            {
                var parts = item.Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    pairs.Add((parts[0].Trim(), parts[1].Trim()));
                }
            }

            return pairs;
        }
    }
}
