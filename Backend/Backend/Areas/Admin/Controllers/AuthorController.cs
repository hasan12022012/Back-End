using Microsoft.AspNetCore.Mvc;

namespace Backend.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
