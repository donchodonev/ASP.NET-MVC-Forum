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

        public static IActionResult RedirectToActionWithErrorMessage(this Controller controller, string errorMessage, string controllerName, string actionName)
        {
            controller.TempData["ErrorMessage"] = errorMessage;
            return controller.RedirectToAction(actionName, controllerName);
        }

        public static IActionResult RedirectToActionWithMessage(this Controller controller, string message, string controllerName, string actionName)
        {
            controller.TempData["Message"] = message;
            return controller.RedirectToAction(actionName, controllerName);
        }
    }
}
