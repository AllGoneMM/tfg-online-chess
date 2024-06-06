using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;

namespace ChessWebApp.TagHelpers
{
    [HtmlTargetElement(Attributes = "asp-active-controller-action")]
    public class ActiveRouteTagHelper : TagHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ActiveRouteTagHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        [HtmlAttributeName("asp-active-controller-action")]
        public string ControllerActionPairs { get; set; }

        [HtmlAttributeName("asp-active-class-add")]
        public string AddClass { get; set; }

        [HtmlAttributeName("asp-active-class-remove")]
        public string RemoveClass { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var routeData = _httpContextAccessor.HttpContext.GetRouteData();
            var currentController = routeData.Values["controller"]?.ToString();
            var currentAction = routeData.Values["action"]?.ToString();

            var controllerActionGroups = ParseControllerActionGroups(ControllerActionPairs);
            var addClasses = ParseClasses(AddClass);
            var removeClasses = ParseClasses(RemoveClass);

            for (int i = 0; i < controllerActionGroups.Count; i++)
            {
                var group = controllerActionGroups[i];
                if (group.Controller == null && group.Actions == null)
                {
                    continue; // Skip empty group
                }

                if (string.Equals(group.Controller, currentController, StringComparison.OrdinalIgnoreCase) &&
                    group.Actions.Contains(currentAction, StringComparer.OrdinalIgnoreCase))
                {
                    // Remove specified classes
                    if (i < removeClasses.Count)
                    {
                        var existingClasses = output.Attributes["class"]?.Value?.ToString();
                        var classesToRemove = removeClasses[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);

                        if (!string.IsNullOrEmpty(existingClasses))
                        {
                            var classList = existingClasses.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
                            foreach (var classToRemove in classesToRemove)
                            {
                                classList.Remove(classToRemove);
                            }

                            var updatedClasses = string.Join(" ", classList);
                            output.Attributes.SetAttribute("class", updatedClasses);
                        }
                    }

                    // Add active classes
                    if (i < addClasses.Count)
                    {
                        var existingClasses = output.Attributes["class"]?.Value?.ToString();
                        var classesToAdd = addClasses[i];
                        var newClasses = string.IsNullOrEmpty(existingClasses) ? classesToAdd : $"{existingClasses} {classesToAdd}";
                        output.Attributes.SetAttribute("class", newClasses);
                    }

                    break; // Apply the first matching group
                }
            }
        }

        private List<ControllerActionGroup> ParseControllerActionGroups(string input)
        {
            var groups = new List<ControllerActionGroup>();
            var items = input?.Split(';', StringSplitOptions.None) ?? Array.Empty<string>();

            foreach (var item in items)
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    groups.Add(new ControllerActionGroup { Controller = null, Actions = null });
                }
                else
                {
                    var parts = item.Split(':', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        var controller = parts[0].Trim();
                        var actions = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim()).ToList();
                        groups.Add(new ControllerActionGroup { Controller = controller, Actions = actions });
                    }
                }
            }

            return groups;
        }

        private List<string> ParseClasses(string input)
        {
            return input?.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim()).ToList() ?? new List<string>();
        }

        private class ControllerActionGroup
        {
            public string Controller { get; set; }
            public List<string> Actions { get; set; }
        }
    }
}
