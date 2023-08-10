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

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Tag dbTag = await GetByIdAsync((int)id);

            ViewBag.Blogs = await GetProductsAsync();

            TagUpdateVM updatedTag = new()
            {
                Name = dbTag.Name,
                Blogs = dbTag.BlogTags
            };

            return View(updatedTag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, TagUpdateVM updatedTag)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            ViewBag.Blogs = await GetProductsAsync();

            if (!ModelState.IsValid) return View(updatedTag);

            Tag dbTag = await GetByIdAsync(id);

            dbTag.Name = updatedTag.Name;
            dbTag.UpdatedAt = DateTime.UtcNow;
            dbTag.UpdatedBy = user.UserName;

            if (updatedTag.BlogIds == null)
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            List<BlogTag> blogs = await _context.BlogTags
                .Where(m => !m.IsDeleted && m.TagId == id)
                .ToListAsync();

            foreach (var blog in blogs)
            {
                dbTag.BlogTags.Remove(blog);
            }

            foreach (var blogId in updatedTag.BlogIds)
            {
                BlogTag blog = new()
                {
                    BlogId = blogId
                };

                dbTag.BlogTags.Add(blog);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //[Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Tag tag = await GetByIdAsync((int)id);

            if (tag == null) return NotFound();

            TagDetailVM tagDetail = new()
            {
                Name = tag.Name,
                Blogs = tag.BlogTags
            };

            return View(tagDetail);
        }

        //[Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            Tag tag = await _context.Tags
                .Where(m => !m.IsDeleted && m.Id == id)
                .Include(m => m.BlogTags)
                .FirstOrDefaultAsync();

            if (tag == null) return NotFound();

            tag.IsDeleted = true;
            tag.DeletedAt = DateTime.UtcNow;
            tag.DeletedBy = user.UserName;

            await _context.SaveChangesAsync();

            return Ok();
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
