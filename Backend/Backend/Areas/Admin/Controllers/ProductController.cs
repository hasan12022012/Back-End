using Backend.DataAccessLayer;
using Backend.Helpers;
using Backend.Models;
using Backend.ViewModels.ProductViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Backend.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public ProductController(AppDbContext context,
            IWebHostEnvironment environment,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _environment = environment;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int page = 1, int take = 3)
        {
            List<Product> products = await _context.Products
                  .Where(m => !m.IsDeleted)
                  .Include(m => m.ProductImages)
                  .OrderByDescending(m => m.Id)
                  .Skip((page * take) - take)
                  .Take(take)
                  .ToListAsync();

            List<ProductListVM> mapDatas = GetMapDatas(products);
            int count = await GetPageCount(take);

            Paginate<ProductListVM> result = new Paginate<ProductListVM>(mapDatas, page, count);

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await GetCategoriesAsync();
            ViewBag.Genres = await GetGenreAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateVM product)
        {
            ViewBag.Categories = await GetCategoriesAsync();
            ViewBag.Genres = await GetGenreAsync();

            foreach (var photo in product.Photos)
            {
                if (!photo.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Photo", "Please, choose correct image type");
                    return View(product);
                }

                if (!photo.CheckFileSize(1000))
                {
                    ModelState.AddModelError("Photo", "Please, choose correct image size");
                    ViewBag.categories = await GetCategoriesAsync();
                    ViewBag.Genres = await GetGenreAsync();
                    return View(product);
                }
            }

            List<ProductImage> images = new List<ProductImage>();

            foreach (var photo in product.Photos)
            {
                string fileName = Guid.NewGuid().ToString() + "_" + photo.FileName;

                string path = FileType.GetFilePath(_environment.WebRootPath, "assets/img", fileName);

                await FileType.SaveFile(path, photo);

                ProductImage image = new ProductImage
                {
                    Name = fileName
                };

                images.Add(image);
            }

            images.FirstOrDefault().IsMain = true;

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            Product newProduct = new()
            {
                ProductImages = images,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                AuthorId = product.AuthorId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = user.UserName
            };

            await _context.ProductImages.AddRangeAsync(images);
            await _context.Products.AddAsync(newProduct);
            await _context.SaveChangesAsync();

            foreach (var genreId in product.GenreIds)
            {
                ProductGenre genre = new()
                {
                    ProductId = newProduct.Id,
                    GenreId = genreId
                };

                await _context.ProductGenres.AddAsync(genre);
            }

            _context.ProductImages.UpdateRange(images);
            _context.Products.Update(newProduct);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Product dbProduct = await GetByIdAsync((int)id);

            ViewBag.categories = await GetCategoriesAsync();
            ViewBag.Genre = await GetGenreAsync();

            //List<ProductGenre> genre = await _context.ProductGenres
            //    .Where(m => m.ProductId == id)
            //    .ToListAsync();

            ProductUpdateVM updatedProduct = new ProductUpdateVM()
            {
                Id = dbProduct.Id,
                Images = dbProduct.ProductImages,
                Name = dbProduct.Name,
                Description = dbProduct.Description,
                Price = dbProduct.Price,
                Author = dbProduct.Author.Name,
                GenreName = dbProduct.ProductGenres,
            };

            return View(updatedProduct);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, ProductUpdateVM updatedProduct)
        {
            ViewBag.categories = await GetCategoriesAsync();
            ViewBag.Genre = await GetGenreAsync();

            if (!ModelState.IsValid) return View(updatedProduct);

            Product dbProduct = await GetByIdAsync(id);

            if (updatedProduct.Photos != null)
            {
                foreach (var photo in updatedProduct.Photos)
                {
                    if (!photo.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("Photo", "Please, choose correct image type");
                        ViewBag.categories = await GetCategoriesAsync();
                        ViewBag.Genre = await GetGenreAsync();
                        return View(updatedProduct);
                    }

                    if (!photo.CheckFileSize(1000))
                    {
                        ModelState.AddModelError("Photo", "Please, choose correct image size");
                        ViewBag.categories = await GetCategoriesAsync();
                        ViewBag.Genre = await GetGenreAsync();
                        return View(updatedProduct);
                    }
                }

                foreach (var photo in updatedProduct.Photos)
                {
                    string fileName = Guid.NewGuid().ToString() + "_" + photo.FileName;

                    string path = FileType.GetFilePath(_environment.WebRootPath, "assets/img", fileName);

                    await FileType.SaveFile(path, photo);

                    ProductImage image = new()
                    {
                        Name = fileName
                    };

                    dbProduct.ProductImages.Add(image);
                }

                ViewBag.categories = await GetCategoriesAsync();
                ViewBag.Genre = await GetGenreAsync();
            }

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            dbProduct.Name = updatedProduct.Name;
            dbProduct.Description = updatedProduct.Description;
            dbProduct.Price = updatedProduct.Price;
            dbProduct.AuthorId = updatedProduct.AuthorId;
            dbProduct.UpdatedAt = DateTime.UtcNow;
            dbProduct.UpdatedBy = user.UserName;

            if (updatedProduct.GenreIds == null)
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            List<ProductGenre> genre = await _context.ProductGenres
                .Where(m => m.ProductId == id)
                .ToListAsync();

            foreach (var item in genre)
            {
                _context.ProductGenres.Remove(item);
            }

            foreach (var genreId in updatedProduct.GenreIds)
            {
                ProductGenre genr = new()
                {
                    GenreId = genreId
                };

                dbProduct.ProductGenres.Add(genr);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProductImage(int? id)
        {
            if (id == null) return BadRequest();

            bool result = false;

            ProductImage productImage = await _context.ProductImages.Where(m => m.Id == id).FirstOrDefaultAsync();

            if (productImage == null) return NotFound();

            Product product = await _context.Products.Include(m => m.ProductImages).FirstOrDefaultAsync(m => m.Id == productImage.ProductId);

            if (product.ProductImages.Count > 1)
            {
                string path = FileType.GetFilePath(_environment.WebRootPath, "assets/img", productImage.Name);

                FileType.DeleteFile(path);

                _context.ProductImages.Remove(productImage);

                await _context.SaveChangesAsync();

                result = true;
            }

            product.ProductImages.FirstOrDefault().IsMain = true;

            await _context.SaveChangesAsync();

            return Ok(result);
        }


        //[Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Product product = await GetByIdAsync((int)id);

            if (product == null) return NotFound();

            ProductDetailVM productDetail = new ProductDetailVM
            {
                Id = product.Id,
                Images = product.ProductImages,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Author = product.Author.Name,
                Genres = product.ProductGenres
            };

            return View(productDetail);
        }

        //[Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            Product product = await _context.Products
                .Where(m => !m.IsDeleted && m.Id == id)
                .Include(m => m.ProductGenres)
                .Include(m => m.ProductImages)
                .Include(m => m.Author)
                .FirstOrDefaultAsync();

            if (product == null) return NotFound();

            foreach (var image in product.ProductImages)
            {
                string path = FileType.GetFilePath(_environment.WebRootPath, "assets/img", image.Name);
                FileType.DeleteFile(path);
                image.IsDeleted = true;
            }

            _context.ProductImages.RemoveRange(product.ProductImages);

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            product.IsDeleted = true;
            product.DeletedAt = DateTime.UtcNow;
            product.DeletedBy = user.UserName;

            await _context.SaveChangesAsync();

            return Ok();
        }

        private async Task<int> GetPageCount(int take)
        {
            int productCount = await _context.Products.Where(m => !m.IsDeleted).CountAsync();

            return (int)Math.Ceiling((decimal)productCount / take);
        }

        private List<ProductListVM> GetMapDatas(List<Product> products)
        {
            List<ProductListVM> productList = new List<ProductListVM>();

            foreach (var product in products)
            {
                ProductListVM newProduct = new ProductListVM
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    MainImage = product.ProductImages.Where(m => m.IsMain).FirstOrDefault()?.Name
                };

                productList.Add(newProduct);
            }

            return productList;
        }

        private async Task<SelectList> GetCategoriesAsync()
        {
            IEnumerable<Author> authors = await _context.Authors.Where(m => !m.IsDeleted).ToListAsync();

            return new SelectList(authors, "Id", "Name");
        }

        private async Task<SelectList> GetGenreAsync()
        {
            IEnumerable<Genre> genres = await _context.Genres.Where(m => !m.IsDeleted).ToListAsync();

            return new SelectList(genres, "Id", "Name");
        }

        private async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products
                .Where(m => !m.IsDeleted && m.Id == id)
                .Include(m => m.ProductGenres)
                .ThenInclude(m => m.Genre)
                .Include(m => m.ProductImages)
                .Include(m => m.Author)
                .FirstOrDefaultAsync();
        }
        private async Task<List<ProductImage>> GetAllByIdAsync(int id)
        {
            return await _context.ProductImages
                .Where(m => !m.IsDeleted && m.ProductId == id)
                .ToListAsync();
        }
    }
}
