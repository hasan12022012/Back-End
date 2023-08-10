using Backend.DataAccessLayer;
using Backend.Models;
using Backend.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    public class BlogController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<AppUser> _userManager;

        public BlogController(AppDbContext appDbContext, UserManager<AppUser> userManager)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? id)
        {
            if (id is null)
            {
                IEnumerable<Blog> blogs = await _appDbContext.Blogs
    .Where(m => !m.IsDeleted)
    .Include(m => m.BlogTags)
    .Include(m => m.BlogCategory)
    .OrderByDescending(m => m.Id)
    .ToListAsync();

                IEnumerable<BlogCategory> blogCategories = await _appDbContext.BlogCategories
                    .Where(m => !m.IsDeleted)
                    .Include(m => m.Blogs)
                    .ToListAsync();

                IEnumerable<Tag> tags = await _appDbContext.Tags
                    .Where(m => !m.IsDeleted)
                    .ToListAsync();

                BlogVM model = new()
                {
                    Blogs = blogs,
                    BlogCategories = blogCategories,
                    Tags = tags
                };

                return View(model);
            }
            else
            {
                IEnumerable<Blog> blogs = await _appDbContext.Blogs
    .Where(m => !m.IsDeleted && m.BlogCategoryId == id)
    .Include(m => m.BlogTags)
    .Include(m => m.BlogCategory)
    .OrderByDescending(m => m.Id)
    .ToListAsync();

                IEnumerable<BlogCategory> blogCategories = await _appDbContext.BlogCategories
                    .Where(m => !m.IsDeleted)
                    .Include(m => m.Blogs)
                    .ToListAsync();

                IEnumerable<Tag> tags = await _appDbContext.Tags
                    .Where(m => !m.IsDeleted)
                    .ToListAsync();

                BlogVM model = new()
                {
                    Blogs = blogs,
                    BlogCategories = blogCategories,
                    Tags = tags
                };

                return View(model);
            }
        }

        public async Task<IActionResult> Search(string search)
        {
            BlogVM searchBlog = new();

            if (search != null)
            {
                searchBlog.Blogs = await _appDbContext.Blogs
                    .Where(m => !m.IsDeleted && m.Name.ToLower()
                    .Contains(search.ToLower()) && !m.IsDeleted)
                    .Include(m => m.BlogTags)
                    .Include(m => m.BlogCategory)
                    .ToListAsync();
            }
            else
            {
                searchBlog.Blogs = await _appDbContext.Blogs
                    .Where(m => !m.IsDeleted)
                    .Include(m => m.BlogTags)
                    .Include(m => m.BlogCategory)
                    .ToListAsync();
            }

            return PartialView("_BlogsPartial", searchBlog);
        }

        public async Task<IActionResult> FilterTag(int id)
        {
            BlogVM model = new();

            if (id != null)
            {
                model.Blogs = await _appDbContext.Blogs
                    .Where(m => !m.IsDeleted && m.BlogTags
                    .Where(m => m.TagId == id)
                    .FirstOrDefault().TagId == id)
                    .Include(m => m.BlogTags)
                    .Include(m => m.BlogCategory)
                    .ToListAsync();
            }
            else
            {
                model.Blogs = await _appDbContext.Blogs
                    .Where(m => !m.IsDeleted)
                    .Include(m => m.BlogTags)
                    .Include(m => m.BlogCategory)
                    .ToListAsync();
            }

            return PartialView("_BlogsPartial", model);
        }

        public async Task<IActionResult> FilterCategory(int id)
        {
            BlogVM model = new();

            if (id != null)
            {
                model.Blogs = await _appDbContext.Blogs
                    .Where(m => !m.IsDeleted && m.BlogCategoryId == id)
                    .Include(m => m.BlogTags)
                    .Include(m => m.BlogCategory)
                    .ToListAsync();
            }
            else
            {
                model.Blogs = await _appDbContext.Blogs
                    .Where(m => !m.IsDeleted)
                    .Include(m => m.BlogTags)
                    .Include(m => m.BlogCategory)
                    .ToListAsync();
            }

            return PartialView("_BlogsPartial", model);
        }

        public async Task<IActionResult> Detail(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
                ViewBag.UserId = user.Id;
            }

            IEnumerable<Blog> blogs = await _appDbContext.Blogs
                .Where(m => !m.IsDeleted)
                .Include(m => m.BlogTags)
                .Include(m => m.BlogCategory)
                .OrderByDescending(m => m.Id)
                .ToListAsync();

            IEnumerable<BlogCategory> blogCategories = await _appDbContext.BlogCategories
                .Where(m => !m.IsDeleted)
                .Include(m => m.Blogs)
                .ToListAsync();

            IEnumerable<Tag> tags = await _appDbContext.Tags
                .Where(m => !m.IsDeleted)
                .ToListAsync();

            Blog blog = await _appDbContext.Blogs
                .Where(m => !m.IsDeleted && m.Id == id)
                .Include(m => m.BlogTags)
                .ThenInclude(m => m.Tag)
                .Include(m => m.BlogCategory)
                .Include(m => m.BlogComments)
                .ThenInclude(m => m.AppUser)
                .FirstOrDefaultAsync();

            IEnumerable<BlogComment> comments = await _appDbContext.BlogComments.Where(m => !m.IsDeleted && m.BlogId == id).ToListAsync();

            BlogVM model = new();
            model.Blog = blog;
            model.Blogs = blogs;
            model.Tags = tags;
            model.BlogComments = comments;
            model.BlogCategories = blogCategories;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(string commentMessage, int productId)
        {
            AppUser user = null;

            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            else
            {
                RedirectToAction("Login", "Account");
            }

            ViewBag.UserId = user.Id;

            BlogComment comment = new();
            comment.BlogId = productId;
            comment.AppUserId = user.Id;
            comment.Message = commentMessage;
            comment.CreateDate = DateTime.Now;

            List<BlogComment> comments = new();
            comments.Add(comment);

            BlogVM model = new()
            {
                BlogComments = comments
            };

            await _appDbContext.BlogComments.AddAsync(comment);
            await _appDbContext.SaveChangesAsync();

            return PartialView("_BlogCommentPartial", model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(int id)
        {
            BlogComment comment = await _appDbContext.BlogComments.FirstOrDefaultAsync(m => m.Id == id);

            _appDbContext.BlogComments.Remove(comment);
            await _appDbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
