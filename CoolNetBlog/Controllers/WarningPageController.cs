using Microsoft.AspNetCore.Mvc;

namespace CoolNetBlog.Controllers
{
    public class WarningPageController : Controller
    {
        public IActionResult Error()
        {
            return View();
        }
    }
}
