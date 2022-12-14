using IdentityAppCourse2022.Data;
using IdentityAppCourse2022.Models;
using IdentityAppCourse2022.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Stripe;
using System.Numerics;

namespace IdentityAppCourse2022.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment webHostEnvironment;
        public ProductController(ApplicationDbContext db, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            webHostEnvironment = hostEnvironment;
        }
        
        public IActionResult Index(string id)
        {
            return View(_db.Product.Where(x => x.provider.Id == id).ToList());
        }

        public IActionResult AllProducts()
        {
             return View(_db.Product.ToList());
        }
        [HttpGet]
        public ActionResult ViewProduct(Models.Product prod)
        {
            var complex = _db.Product.FirstOrDefault(u => u.Id == prod.Id);
            ProductViewModel speakerViewModel = new ProductViewModel();
            speakerViewModel.Id = complex.Id;
            speakerViewModel.Name = complex.Name;
            speakerViewModel.Description = complex.Description;
            speakerViewModel.Price = complex.Price;
            //var providerDB = _db.Product.Where(b => b.provider == complex.provider).FirstOrDefault();
            //speakerViewModel.Provider = prod.provider.UserName;
            //speakerViewModel.CategorySelected = complex.category.Name;
            speakerViewModel.ExistingImage = complex.ProfilePicture;
            return View(speakerViewModel);
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
                return RedirectToAction(nameof(AllProducts));
            }
            if (string.IsNullOrEmpty(product.Id))
            {
                //create
                string uniqueFileName = ProcessUploadedFile(product);
                var categoryDB = _db.Category.Where(c => c.Id == product.CategorySelected).FirstOrDefault();
                var providerDB = _db.AppUser.Where(p => p.Id == product.Provider).FirstOrDefault();
                if (categoryDB != null && providerDB != null)
                {
                    _db.Product.Add(new Models.Product { Name = product.Name, Description = product.Description, Price = product.Price, category = categoryDB, provider = providerDB, ProfilePicture = uniqueFileName });
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
                    return RedirectToAction(nameof(AllProducts));
                }
                productDb.Name = product.Name;
                productDb.Description = product.Description;
                productDb.Price = product.Price;
                productDb.category = categoryDB;
                if (product.SpeakerPicture != null)
                {
                    productDb.ProfilePicture = null;
                    productDb.ProfilePicture = ProcessUploadedFile(product);
                }
                _db.Product.Update(productDb);
                _db.SaveChanges();
            }
            return RedirectToAction(nameof(AllProducts));

        }
        
        
        private string ProcessUploadedFile(ViewModels.ProductViewModel model)
        {
            string uniqueFileName = null;

            if (model.SpeakerPicture != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "Uploads");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.SpeakerPicture.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.SpeakerPicture.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}
