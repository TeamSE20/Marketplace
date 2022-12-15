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
using System.Security.Claims;

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddText(string Id, string Text)
        {
            ClaimsPrincipal currentUser = this.User;

            if (Text != null && User.IsInRole("Client"))
            {
                var review = new Models.Review();

                {
                    review.Text = Text;
                    review.clientId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
                    review.productId = Id;
                    review.dateReview = DateTime.Now;
                }

                _db.Reviews.Add(review);
                _db.SaveChanges();
            }
            
            var prod =_db.Product.FirstOrDefault(m => m.Id == Id);
            var complex = _db.Product.FirstOrDefault(m => m.Id == prod.Id);
            ReviewViewModel speakerViewModel = new ReviewViewModel() { reviewList = new List<Models.Review>() };
            speakerViewModel.Id = complex.Id;
            speakerViewModel.Name = complex.Name;
            speakerViewModel.Description = complex.Description;
            speakerViewModel.Price = complex.Price;
            //var providerDB = _db.Product.Where(b => b.provider == complex.provider).FirstOrDefault();
            var provider = _db.AppUser.FirstOrDefault(m => m.Id == complex.providerId);
            speakerViewModel.Provider = provider.UserName;
            //speakerViewModel.Provider = prod.provider.UserName;
            var category = _db.Category.FirstOrDefault(m => m.Id == complex.categoryId);
            speakerViewModel.CategorySelected = category.Name;
            speakerViewModel.ExistingImage = complex.ProfilePicture;
            speakerViewModel.Text = null;
            speakerViewModel.client = null;
            //speakerViewModel.reviewList = null;
            foreach (var review in _db.Reviews)
            {
                if (review.productId == speakerViewModel.Id)
                {

                    var view = new Models.Review();
                    view.Id = review.Id;
                    view.Text = review.Text;
                    view.clientId = review.clientId;
                    view.productId = review.productId;
                    view.dateReview = review.dateReview;
                    speakerViewModel.reviewList.Add(view);
                }
            }
            return View(speakerViewModel);
        }
        [HttpGet]
        public ActionResult ViewProduct(Models.Product prod)
        {
            //var complex = _db.Product.Join(_db.AppUser, prov => prov.provider, pr => pr.Id, (piv) => new {piv}).FirstOrDefault(u => u.Id == prod.Id);
            var complex = _db.Product.FirstOrDefault(m => m.Id == prod.Id);
            ReviewViewModel speakerViewModel = new ReviewViewModel() { reviewList = new List<Models.Review>() };
            speakerViewModel.Id = complex.Id;
            speakerViewModel.Name = complex.Name;
            speakerViewModel.Description = complex.Description;
            speakerViewModel.Price = complex.Price;
            //var providerDB = _db.Product.Where(b => b.provider == complex.provider).FirstOrDefault();
            var provider = _db.AppUser.FirstOrDefault(m => m.Id == complex.providerId);
            speakerViewModel.Provider = provider.UserName;
            //speakerViewModel.Provider = prod.provider.UserName;
            var category = _db.Category.FirstOrDefault(m => m.Id == complex.categoryId);
            speakerViewModel.CategorySelected = category.Name;
            speakerViewModel.ExistingImage = complex.ProfilePicture;
            speakerViewModel.Text = null;
            speakerViewModel.client = null;
            //speakerViewModel.reviewList = null;
            foreach (var review in _db.Reviews)
            {
                if (review.productId == speakerViewModel.Id)
                {

                    var view = new Models.Review();
                    view.Id = review.Id;
                    view.Text = review.Text;
                    view.clientId = review.clientId;
                    view.productId = review.productId;
                    view.dateReview = review.dateReview;
                    speakerViewModel.reviewList.Add(view);
                }
            }
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
            if (string.IsNullOrEmpty(product.Id))
            {
                //create
                string uniqueFileName = ProcessUploadedFile(product);
                var categoryDB = _db.Category.Where(c => c.Id == product.CategorySelected).FirstOrDefault();
                var providerDB = _db.AppUser.Where(p => p.Id == product.Provider).FirstOrDefault();
                if (categoryDB != null && providerDB != null)
                {
                    _db.Product.Add(new Models.Product { Name = product.Name, Description = product.Description, Price = product.Price, category = categoryDB, provider = providerDB, providerId=providerDB.Id, categoryId = categoryDB.Id, ProfilePicture = uniqueFileName });
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
                        if (productExist != null)
            {
                //error
                return RedirectToAction(nameof(AllProducts));
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
