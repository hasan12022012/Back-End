using Microsoft.AspNetCore.Mvc;

namespace Backend.Areas.Admin.Controllers
{
    public class BlogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
