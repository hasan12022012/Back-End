using Backend.DataAccessLayer;
using Backend.Helpers;
using Backend.Models;
using Backend.ViewModels.AuthorViewModels;
using Backend.ViewModels.BlogViewModels;
using Backend.ViewModels.GenreViewModels;
using Backend.ViewModels.ProductViewModels;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Backend.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public BlogController(AppDbContext context,
            UserManager<AppUser> userManager,
            IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        public async Task<IActionResult> Index(int page = 1, int take = 3)
        {
            List<Blog> blogs = await _context.Blogs
                  .Where(m => !m.IsDeleted)
                  .Include(m => m.BlogTags)
                  .ThenInclude(m => m.Tag)
                  .Include(m => m.BlogCategory)
                  .OrderByDescending(m => m.Id)
                  .Skip((page * take) - take)
                  .Take(take)
                  .ToListAsync();

            List<BlogListVM> mapDatas = GetMapDatas(blogs);
            int count = await GetPageCount(take);

            Paginate<BlogListVM> result = new(mapDatas, page, count);

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await GetCategoriesAsync();
            ViewBag.Tags = await GetTagAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogCreateVM blog)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            ViewBag.Categories = await GetCategoriesAsync();
            ViewBag.Tags = await GetTagAsync();

            if (!blog.Photo.CheckFileType("image/"))
            {
                ModelState.AddModelError("Photo", "Please, choose correct image type");
                ViewBag.Categories = await GetCategoriesAsync();
                ViewBag.Tags = await GetTagAsync();
                return View(blog);
            }

            if (!blog.Photo.CheckFileSize(1000))
            {
                ModelState.AddModelError("Photo", "Please, choose correct image size");
                ViewBag.Categories = await GetCategoriesAsync();
                ViewBag.Tags = await GetTagAsync();
                return View(blog);
            }

            string fileName = Guid.NewGuid().ToString() + "_" + blog.Photo.FileName;

            string path = FileType.GetFilePath(_environment.WebRootPath, "assets/img", fileName);

            await FileType.SaveFile(path, blog.Photo);

            Blog newBlog = new()
            {
                Name = blog.Name,
                Description = blog.Description,
                Image = fileName,
                BlogCategoryId = blog.CategoryId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = user.UserName
            };

            await _context.Blogs.AddAsync(newBlog);
            await _context.SaveChangesAsync();

            foreach (var tagId in blog.TagIds)
            {
                BlogTag blogtag = new()
                {
                    TagId = tagId,
                    BlogId = newBlog.Id
                };

                await _context.BlogTags.AddAsync(blogtag);
            }

            _context.Blogs.Update(newBlog);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Blog dbBlog = await GetByIdAsync((int)id);

            ViewBag.Categories = await GetCategoriesAsync();
            ViewBag.Tags = await GetTagAsync();

            BlogUpdateVM updatedBlog = new()
            {
                Id = dbBlog.Id,
                Name = dbBlog.Name,
                Description = dbBlog.Description,
                Image = dbBlog.Image,
                Tags = dbBlog.BlogTags
            };

            return View(updatedBlog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, BlogUpdateVM updatedBlog)
        {
            ViewBag.Categories = await GetCategoriesAsync();
            ViewBag.Tags = await GetTagAsync();

            if (!ModelState.IsValid) return View(updatedBlog);

            Blog dbBlog = await GetByIdAsync(id);

            if (updatedBlog.Photo != null)
            {
                if (!updatedBlog.Photo.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Photo", "Please, choose correct image type");
                    ViewBag.Categories = await GetCategoriesAsync();
                    ViewBag.Tags = await GetTagAsync();
                    return View(updatedBlog);
                }

                if (!updatedBlog.Photo.CheckFileSize(1000))
                {
                    ModelState.AddModelError("Photo", "Please, choose correct image size");
                    ViewBag.Categories = await GetCategoriesAsync();
                    ViewBag.Tags = await GetTagAsync();
                    return View(updatedBlog);
                }

                string fileName = Guid.NewGuid().ToString() + "_" + updatedBlog.Photo.FileName;

                string path = FileType.GetFilePath(_environment.WebRootPath, "assets/img", fileName);

                await FileType.SaveFile(path, updatedBlog.Photo);

                dbBlog.Image = fileName;
            }

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            dbBlog.Name = updatedBlog.Name;
            dbBlog.Description = updatedBlog.Description;
            dbBlog.BlogCategoryId = updatedBlog.CategoryId;
            dbBlog.UpdatedAt = DateTime.UtcNow;
            dbBlog.UpdatedBy = user.UserName;

            if (updatedBlog.TagIds == null)
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            List<BlogTag> blogTags = await _context.BlogTags
                 .Where(m => !m.IsDeleted && m.BlogId == id)
                 .ToListAsync();

            foreach (var item in blogTags)
            {
                dbBlog.BlogTags.Remove(item);
            }

            foreach (var tagId in updatedBlog.TagIds)
            {
                BlogTag tag = new()
                {
                    TagId = tagId
                };

                dbBlog.BlogTags.Add(tag);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //[Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Blog blog = await GetByIdAsync((int)id);

            if (blog == null) return NotFound();

            BlogDetailVM blogDetail = new()
            {
                Id = blog.Id,
                Image = blog.Image,
                Name = blog.Name,
                Description = blog.Description,
                Category = blog.BlogCategory.Name,
                Tags = blog.BlogTags
            };

            return View(blogDetail);
        }

        //[Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            Blog blog = await _context.Blogs
                .Where(m => !m.IsDeleted && m.Id == id)
                .Include(m => m.BlogTags)
                .Include(m => m.BlogCategory)
                .FirstOrDefaultAsync();

            if (blog == null) return NotFound();

            string path = FileType.GetFilePath(_environment.WebRootPath, "assets/img", blog.Image);
            FileType.DeleteFile(path);

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            blog.IsDeleted = true;
            blog.DeletedAt = DateTime.UtcNow;
            blog.DeletedBy = user.UserName;

            await _context.SaveChangesAsync();

            return Ok();
        }

        private async Task<int> GetPageCount(int take)
        {
            int blogCount = await _context.Blogs.Where(m => !m.IsDeleted).CountAsync();

            return (int)Math.Ceiling((decimal)blogCount / take);
        }

        private List<BlogListVM> GetMapDatas(List<Blog> blogs)
        {
            List<BlogListVM> blogList = new();

            foreach (var blog in blogs)
            {
                BlogListVM newBlog = new()
                {
                    Id = blog.Id,
                    Name = blog.Name,
                    Description = blog.Description,
                    Image = blog.Image
                };

                blogList.Add(newBlog);
            }

            return blogList;
        }

        private async Task<SelectList> GetCategoriesAsync()
        {
            IEnumerable<BlogCategory> categories = await _context.BlogCategories.Where(m => !m.IsDeleted).ToListAsync();

            return new SelectList(categories, "Id", "Name");
        }

        private async Task<SelectList> GetTagAsync()
        {
            IEnumerable<Tag> tags = await _context.Tags.Where(m => !m.IsDeleted).ToListAsync();

            return new SelectList(tags, "Id", "Name");
        }

        private async Task<Blog> GetByIdAsync(int id)
        {
            return await _context.Blogs
                .Where(m => !m.IsDeleted && m.Id == id)
                .Include(m => m.BlogTags)
                .ThenInclude(m => m.Tag)
                .Include(m => m.BlogCategory)
                .FirstOrDefaultAsync();
        }
    }
}
