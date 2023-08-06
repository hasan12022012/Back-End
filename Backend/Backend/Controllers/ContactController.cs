using Backend.DataAccessLayer;
using Backend.Models;
using Backend.Services;
using Backend.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    public class ContactController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly LayoutService _layoutService;

        public ContactController(AppDbContext appDbContext, LayoutService layoutService)
        {
            _appDbContext = appDbContext;
            _layoutService = layoutService;
        }

        public async Task<IActionResult> Index()
        {
            Dictionary<string, string> settings = await _layoutService.GetDatasFromSetting();

            ContactVM model = new()
            {
                Contact = new Contact(),
                Settings = settings
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Contact contact)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(contact);
                }

                bool isExist = await _appDbContext.Contacts.AnyAsync(m =>
                m.Email.Trim() == contact.Email.Trim());

                if (isExist)
                {
                    ModelState.AddModelError("Email", "Email already exist!");
                }

                await _appDbContext.Contacts.AddAsync(contact);
                await _appDbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View();
            }
        }
    }
}
