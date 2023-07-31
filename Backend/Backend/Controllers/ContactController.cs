using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
