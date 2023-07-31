using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    public class BasketController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
