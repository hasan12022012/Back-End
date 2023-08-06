using Backend.DataAccessLayer;
using Backend.Models;
using Backend.Services;
using Backend.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

        public async Task<int> GetUserBasketProductsCount(ClaimsPrincipal userClaims)
        {
            var user = await _userManager.GetUserAsync(userClaims);
            if (user == null) return 0;
            var basketProductCount = await _context.BasketProducts.Where(m => m.Basket.AppUserId == user.Id).SumAsync(m => m.Quantity);
            return basketProductCount;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            Dictionary<string, string> settings = await _layoutService.GetDatasFromSetting();

            HeaderVM model = new HeaderVM
            {
                Settings = settings,
                Count = await GetUserBasketProductsCount(_httpContextAccessor.HttpContext.User)
            };

            return await Task.FromResult(View(model));
        }
    }
}
