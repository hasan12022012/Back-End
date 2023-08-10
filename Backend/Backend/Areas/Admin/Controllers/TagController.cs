using Backend.DataAccessLayer;
using Backend.Helpers;
using Backend.Models;
using Backend.ViewModels.TagViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Backend.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TagController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public TagController(AppDbContext context,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int page = 1, int take = 3)
        {
            List<Tag> tags = await _context.Tags
                  .Where(m => !m.IsDeleted)
                  .Include(m => m.BlogTags)
                  .ThenInclude(m => m.Blog)
                  .OrderByDescending(m => m.Id)
                  .Skip((page * take) - take)
                  .Take(take)
                  .ToListAsync();

            List<TagListVM> mapDatas = GetMapDatas(tags);
            int count = await GetPageCount(take);

            Paginate<TagListVM> result = new(mapDatas, page, count);

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Blogs = await GetProductsAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TagCreateVM tag)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            ViewBag.Blogs = await GetProductsAsync();

            Tag newTag = new()
            {
                Name = tag.Name,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = user.UserName
            };

            await _context.Tags.AddAsync(newTag);
            await _context.SaveChangesAsync();

            foreach (var blogId in tag.BlogIds)
            {
                BlogTag blog = new()
                {
                    TagId = newTag.Id,
                    BlogId = blogId
                };

                await _context.BlogTags.AddAsync(blog);
            }

            _context.Tags.Update(newTag);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task<int> GetPageCount(int take)
        {
            int tagCount = await _context.Tags.Where(m => !m.IsDeleted).CountAsync();

            return (int)Math.Ceiling((decimal)tagCount / take);
        }

        private List<TagListVM> GetMapDatas(List<Tag> tags)
        {
            List<TagListVM> tagList = new();

            foreach (var tag in tags)
            {
                TagListVM newTag = new()
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    Blogs = tag.BlogTags,
                };

                tagList.Add(newTag);
            }

            return tagList;
        }

        private async Task<SelectList> GetProductsAsync()
        {
            IEnumerable<Blog> blogs = await _context.Blogs.Where(m => !m.IsDeleted).ToListAsync();

            return new SelectList(blogs, "Id", "Name");
        }

        private async Task<Tag> GetByIdAsync(int id)
        {
            return await _context.Tags
                .Where(m => !m.IsDeleted && m.Id == id)
                .Include(m => m.BlogTags)
                .ThenInclude(m => m.Blog)
                .FirstOrDefaultAsync();
        }

    }
}
