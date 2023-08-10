using Backend.DataAccessLayer;
using Backend.Helpers;
using Backend.Models;
using Backend.ViewModels.SliderViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SliderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public SliderController(AppDbContext context,
            IWebHostEnvironment environment,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _environment = environment;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int page = 1, int take = 3)
        {
            ViewBag.Count = await _context.Sliders
                .Where(m => !m.IsDeleted)
                .CountAsync();

            List<Slider> sliders = await _context.Sliders
                  .Where(m => !m.IsDeleted)
                  .OrderByDescending(m => m.Id)
                  .Skip((page * take) - take)
                  .Take(take)
                  .ToListAsync();

            List<SliderListVM> mapDatas = GetMapDatas(sliders);
            int count = await GetPageCount(take);

            Paginate<SliderListVM> result = new(mapDatas, page, count);

            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderCreateVM slider)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (!slider.Photo.CheckFileType("image/"))
            {
                ModelState.AddModelError("Photo", "Please, choose correct image type");
                return View(slider);
            }

            if (!slider.Photo.CheckFileSize(1000))
            {
                ModelState.AddModelError("Photo", "Please, choose correct image size");
                return View(slider);
            }

            string fileName = Guid.NewGuid().ToString() + "_" + slider.Photo.FileName;

            string path = FileType.GetFilePath(_environment.WebRootPath, "assets/img", fileName);

            await FileType.SaveFile(path, slider.Photo);

            Slider newSlider = new()
            {
                Title = slider.Title,
                Content = slider.Content,
                Image = fileName,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = user.UserName
            };

            await _context.Sliders.AddAsync(newSlider);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Slider dbSlider = await GetByIdAsync((int)id);

            SliderUpdateVM updatedSlider = new()
            {
                Id = dbSlider.Id,
                Title = dbSlider.Title,
                Content = dbSlider.Content,
                Image = dbSlider.Image
            };

            return View(updatedSlider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, SliderUpdateVM updatedSlider)
        {
            if (!ModelState.IsValid) return View(updatedSlider);

            Slider dbSlider = await GetByIdAsync(id);

            if (updatedSlider.Photo != null)
            {
                if (!updatedSlider.Photo.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Photo", "Please, choose correct image type");
                    return View(updatedSlider);
                }

                if (!updatedSlider.Photo.CheckFileSize(1000))
                {
                    ModelState.AddModelError("Photo", "Please, choose correct image size");
                    return View(updatedSlider);
                }

                string fileName = Guid.NewGuid().ToString() + "_" + updatedSlider.Photo.FileName;

                string path = FileType.GetFilePath(_environment.WebRootPath, "assets/img", fileName);

                await FileType.SaveFile(path, updatedSlider.Photo);

                dbSlider.Image = fileName;
            }

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            dbSlider.Title = updatedSlider.Title;
            dbSlider.Content = updatedSlider.Content;
            dbSlider.UpdatedAt = DateTime.UtcNow;
            dbSlider.UpdatedBy = user.UserName;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //[Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Slider slider = await GetByIdAsync((int)id);

            if (slider == null) return NotFound();

            SliderDetailVM sliderDetail = new()
            {
                Id = slider.Id,
                Image = slider.Image,
                Title = slider.Title,
                Content = slider.Content,
            };

            return View(sliderDetail);
        }

        //[Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            Slider slider = await _context.Sliders
                .Where(m => !m.IsDeleted && m.Id == id)
                .FirstOrDefaultAsync();

            if (slider == null) return NotFound();

            string path = FileType.GetFilePath(_environment.WebRootPath, "assets/img", slider.Image);
            FileType.DeleteFile(path);

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            slider.IsDeleted = true;
            slider.DeletedAt = DateTime.UtcNow;
            slider.DeletedBy = user.UserName;

            await _context.SaveChangesAsync();

            return Ok();
        }

        private async Task<int> GetPageCount(int take)
        {
            int sliderCount = await _context.Sliders.Where(m => !m.IsDeleted).CountAsync();

            return (int)Math.Ceiling((decimal)sliderCount / take);
        }

        private List<SliderListVM> GetMapDatas(List<Slider> sliders)
        {
            List<SliderListVM> sliderList = new();

            foreach (var slider in sliders)
            {
                SliderListVM newSlider = new()
                {
                    Id = slider.Id,
                    Title = slider.Title,
                    Content = slider.Content,
                    Image = slider.Image
                };

                sliderList.Add(newSlider);
            }

            return sliderList;
        }

        private async Task<Slider> GetByIdAsync(int id)
        {
            return await _context.Sliders
                .Where(m => !m.IsDeleted && m.Id == id)
                .FirstOrDefaultAsync();
        }
    }
}
