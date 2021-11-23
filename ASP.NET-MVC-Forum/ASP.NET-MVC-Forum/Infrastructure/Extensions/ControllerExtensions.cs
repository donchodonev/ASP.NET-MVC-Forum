namespace ASP.NET_MVC_Forum.Infrastructure.Extensions
{
    using Microsoft.AspNetCore.Mvc;

    public static class ControllerExtensions
    {
        public static IActionResult ViewWithErrorMessage(this Controller controller, string errorMessage)
        {
            controller.TempData["ErrorMessage"] = errorMessage;
            return controller.View();
        }
    }
}
