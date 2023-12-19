using IdentityAppCourse2022.Data;
using IdentityAppCourse2022.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using System.Diagnostics;
using System.Linq.Expressions;

namespace IdentityAppCourse2022.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly AlmurutStoreDbContext _almurut;
        private readonly AsiaStoreDbContext _asia;
        private readonly KivanoDbContext _kivano;
        private readonly SoftTechDbContext _softTech;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, AlmurutStoreDbContext almurut, KivanoDbContext kivano, SoftTechDbContext softTech, AsiaStoreDbContext asia)
        {
            _logger = logger;
            _db = db;
            _almurut = almurut;
            _asia = asia;
            _kivano = kivano;
            _softTech = softTech;
        }

        public async Task<IActionResult> Index(string searchString, string sortOrder)
        {
            //ViewData["CurrentFilter"] = searchString;
            //ViewData["CurrentSort"] = sortOrder;

            //var almurutProducts = await _almurut.Product.Select(p => new SearchProduct
            //{
            //    Id = p.Id.ToString(),
            //    Name = p.Name,
            //    Description = p.Description,
            //    Price = p.Price,
            //    DisplayImage = p.DisplayImage,
            //    StoreName = "Almurut Store"
            //}).ToListAsync();

            //var asiaProducts = await _asia.Product.Select(p => new SearchProduct
            //{
            //    Id = p.Id.ToString(),
            //    Name = p.Name,
            //    Description = p.Description,
            //    Price = p.Price,
            //    DisplayImage = p.DisplayImage,
            //    StoreName = "Asia Store"
            //}).ToListAsync();

            //var kivanoProducts = await _kivano.Product.Select(p => new SearchProduct
            //{
            //    Id = p.Id.ToString(),
            //    Name = p.Name,
            //    Description = p.Description,
            //    Price = p.Price,
            //    DisplayImage = p.DisplayImage,
            //    StoreName = "Kivano Store"
            //}).ToListAsync();

            //var softTechProducts = await _softTech.Product.Select(p => new SearchProduct
            //{
            //    Id = p.Id.ToString(),
            //    Name = p.Name,
            //    Description = p.Description,
            //    Price = p.Price,
            //    DisplayImage = p.DisplayImage,
            //    StoreName = "SoftTech Store"
            //}).ToListAsync();

            //var allProducts = almurutProducts
            //    .Concat(asiaProducts)
            //    .Concat(kivanoProducts)
            //    .Concat(softTechProducts);

            //if (!string.IsNullOrEmpty(searchString))
            //{
            //    allProducts = allProducts.Where(p =>
            //        p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
            //        p.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase));
            //}

            //ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            //ViewBag.DescriptionSortParm = sortOrder == "description" ? "description_desc" : "description";
            //ViewBag.PriceSortParm = sortOrder == "price" ? "price_desc" : "price";

            //allProducts = SortProducts(allProducts.AsQueryable(), sortOrder);

            //return View(allProducts.ToList());
            return View(_db.Product.Where(x => x.IsDeleted == false).ToList());
        }



        private static IQueryable<SearchProduct> SortProducts(IQueryable<SearchProduct> products, string sortOrder)
        {
            return sortOrder switch
            {
                "name_desc" => products.OrderByDescending(p => p.Name),
                "description" => products.OrderBy(p => p.Description),
                "description_desc" => products.OrderByDescending(p => p.Description),
                "price" => products.OrderBy(p => p.Price),
                "price_desc" => products.OrderByDescending(p => p.Price),
                _ => products.OrderBy(p => p.Name),
            };
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Help()
        {
            return View();
        }
    }
}