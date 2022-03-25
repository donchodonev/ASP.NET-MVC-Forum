namespace ASP.NET_MVC_Forum.Web.Infrastructure.Extensions
{
    using Microsoft.AspNetCore.Mvc;
    using static ASP.NET_MVC_Forum.Domain.Constants.ClientMessage.MessageType;

    public static class ControllerExtensions
    {
        public static ActionResult ViewWithErrorMessage(this Controller controller, string errorMessage)
        {
            controller.TempData[ErrorMessage] = errorMessage;
            return controller.View();
        }

        public static ActionResult RedirectToActionWithErrorMessage(this Controller controller,
            string errorMessage,
            string controllerName,
            string actionName)
        {
            controller.TempData[ErrorMessage] = errorMessage;
            return controller.RedirectToAction(actionName, controllerName);
        }

        public static ActionResult RedirectToActionWithErrorMessage(this Controller controller,
            string errorMessage,
            string controllerName,
            string actionName,
            object routeValues)
        {
            controller.TempData[ErrorMessage] = errorMessage;
            return controller.RedirectToAction(actionName, controllerName, routeValues);
        }

        public static ActionResult RedirectToActionWithGenericMessage(this Controller controller, 
            string genericMessage, 
            string controllerName, 
            string actionName)
        {
            controller.TempData[GenericMessage] = genericMessage;
            return controller.RedirectToAction(actionName, controllerName);
        }

        public static ActionResult RedirectToActionWithSuccessMessage(this Controller controller,
            string successMessage,
            string controllerName,
            string actionName)
        {
            controller.TempData[SuccessMessage] = successMessage;
            return controller.RedirectToAction(actionName, controllerName);
        }

        public static ActionResult RedirectToActionWithSuccessMessage(this Controller controller,
            string successMessage,
            string controllerName,
            string actionName,
            object routeValues)
        {
            controller.TempData[SuccessMessage] = successMessage;
            return controller.RedirectToAction(actionName, controllerName, routeValues);
        }
    }
}
