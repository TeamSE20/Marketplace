using IdentityAppCourse2022.Data;
using IdentityAppCourse2022.Models;
using IdentityAppCourse2022.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IdentityAppCourse2022.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProductController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(string? id)
        {
            if(String.IsNullOrEmpty(id))
            {
                return View(_db.Product.ToList());
            }
            else
            {
                return View("AllProducts", _db.Product.Where(x => x.provider.Id == id).ToList());
            }
        }

        [HttpGet]
        public ActionResult Upsert(string id)
        {
            var categories = _db.Category.ToList();
            List<SelectListItem> listItems = new List<SelectListItem>();
            foreach (var category in categories)
            {
                listItems.Add(new SelectListItem()
                {
                    Value = category.Id,
                    Text = category.Name,
                });
            }

            ProductViewModel productViewModel = new ProductViewModel();
            productViewModel.CategoryList = listItems;
            if (String.IsNullOrEmpty(id))
            {
                return View(productViewModel);
            }
            else
            {
                //update
                var product = _db.Product.FirstOrDefault(u => u.Id == id);
                productViewModel.Id = product.Id;
                productViewModel.Name = product.Name;
                productViewModel.Description = product.Description;
                productViewModel.Price = product.Price;
                productViewModel.CategorySelected = product.category.Id;
                return View(productViewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ViewModels.ProductViewModel product)
        {
            var products = _db.Product.ToList();
            var productExist = products.Where(p => p.Name == product.Name).FirstOrDefault();
            if (productExist != null)
            {
                //error
                return RedirectToAction(nameof(Index));
            }
            if (string.IsNullOrEmpty(product.Id))
            {
                //create
                var categoryDB = _db.Category.Where(c => c.Id == product.CategorySelected).FirstOrDefault();
                var providerDB = _db.AppUser.Where(p => p.Id == product.Provider).FirstOrDefault();
                if (categoryDB != null && providerDB != null)
                {
                    _db.Product.Add(new Product { Name = product.Name, Description = product.Description, Price = product.Price, category = categoryDB, provider = providerDB });
                    _db.SaveChanges();
                }
            }
            else
            {
                //update
                var productDb = _db.Product.FirstOrDefault(u => u.Id == product.Id);
                var categoryDB = _db.Category.Where(c => c.Id == product.CategorySelected).FirstOrDefault();
                if (productDb == null)
                {
                    return RedirectToAction(nameof(Index));
                }
                productDb.Name = product.Name;
                productDb.Description = product.Description;
                productDb.Price = product.Price;
                productDb.category = categoryDB;
                _db.Product.Update(productDb);
                _db.SaveChanges();
            }
            return RedirectToAction(nameof(Index));

        }
    }
}
