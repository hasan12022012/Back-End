using Backend.DataAccessLayer;
using Backend.Models;
using Backend.Services;
using Backend.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Backend.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly LayoutService _layoutService;
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public HeaderViewComponent(LayoutService layoutService,
            AppDbContext context,
            UserManager<AppUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _layoutService = layoutService;
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            Dictionary<string, string> settings = await _layoutService.GetDatasFromSetting();

            HeaderVM model = new HeaderVM
            {
                Settings = settings,
            };

            return await Task.FromResult(View(model));
        }
    }
}
