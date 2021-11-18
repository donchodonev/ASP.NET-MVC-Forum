namespace ASP.NET_MVC_Forum.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]

    public class ChatController : Controller
    {
        public IActionResult Chat()
        {
            return this.View();
        }
    }
}
