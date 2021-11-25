namespace ASP.NET_MVC_Forum.Infrastructure.Extensions
{
    using Microsoft.AspNetCore.Mvc;
    using static ASP.NET_MVC_Forum.Data.Constants.ClientMessage.MessageType;

    public static class ControllerExtensions
    {
        public static IActionResult ViewWithErrorMessage(this Controller controller, string errorMessage)
        {
            controller.TempData["ErrorMessage"] = errorMessage;
            return controller.View();
        }

        public static IActionResult RedirectToActionWithErrorMessage(this Controller controller, string errorMessage, string controllerName, string actionName)
        {
            controller.TempData[ErrorMessage] = errorMessage;
            return controller.RedirectToAction(actionName, controllerName);
        }

        public static IActionResult RedirectToActionWithGenericMessage(this Controller controller, string genericMessage, string controllerName, string actionName)
        {
            controller.TempData[GenericMessage] = genericMessage;
            return controller.RedirectToAction(actionName, controllerName);
        }

        public static IActionResult RedirectToActionWithSuccessMessage(this Controller controller, string successMessage, string controllerName, string actionName)
        {
            controller.TempData[SuccessMessage] = successMessage;
            return controller.RedirectToAction(actionName, controllerName);
        }
    }
}
