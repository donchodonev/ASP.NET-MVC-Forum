namespace ASP.NET_MVC_Forum.Web.Controllers
{
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Web.Extensions.ControllerExtensions;

    public class ErrorController : Controller
    {
        public async Task<IActionResult> Show()
        {
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            return this.RedirectToActionWithErrorMessage(exceptionDetails.Error.Message, "Home", "Index");
        }
    }
}
