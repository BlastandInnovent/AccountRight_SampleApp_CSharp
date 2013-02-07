using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace CSharpSamples.Common.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString ToggleButtons(this HtmlHelper helper, string id, IEnumerable<SelectListItem> items)
        {
            var toggleButtons = "<div class=\"controls\">"
                    + "<div class=\"btn-group\" id=\"" + id + "\" data-toggle=\"buttons-radio\">";

            foreach (var item in items)
            {
                toggleButtons += "<button type=\"button\" class=\"btn" + (item.Selected ? " active" : "") + "\" value=\"" + item.Value + "\">" + item.Text + "</button>";
            }
            toggleButtons += "</div>"
                          + "</div>";
            return new MvcHtmlString(toggleButtons);
        }

        public static MvcHtmlString MenuItem(this HtmlHelper helper, string text, string action, string controller)
        {
            var li = new TagBuilder("li");
            var routeData = helper.ViewContext.RouteData;
            var currentAction = routeData.GetRequiredString("action");
            var currentController = routeData.GetRequiredString("controller");
            if (string.Equals(currentController, controller, StringComparison.OrdinalIgnoreCase))
            {
                li.AddCssClass("active");
            }
            li.InnerHtml = helper.ActionLink(text, action, controller).ToHtmlString();
            return MvcHtmlString.Create(li.ToString());
        }
    }
}
