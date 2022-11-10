using IdentityAppCourse2022.Data;
using IdentityAppCourse2022.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityAppCourse2022.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Authorize(Policy = "Admin")]
        public IActionResult Index()
        {
            var categories = _db.Category.ToList();
            return View(categories);
        }

        [Authorize(Policy = "Admin")]
        [HttpGet]
        public ActionResult Upsert(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return View();
            }
            else
            {
                //update
                var category = _db.Category.FirstOrDefault(u => u.Id == id);
                return View(category);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Category category)
        {
            var categories = _db.Category.ToList();
            var categoryExist = categories.Where(c => c.Name == category.Name).FirstOrDefault();
            if (categoryExist != null)
            {
                //error
                return RedirectToAction(nameof(Index));
            }
            if (string.IsNullOrEmpty(category.Id))
            {
                //create
                _db.Category.Add(new Category { Name = category.Name });
                _db.SaveChanges();
            }
            else
            {
                //update
                var categoryDb = _db.Category.FirstOrDefault(u => u.Id == category.Id);
                if (categoryDb == null)
                {
                    return RedirectToAction(nameof(Index));
                }
                categoryDb.Name = category.Name;
                _db.Category.Update(categoryDb);
                _db.SaveChanges();
            }
            return RedirectToAction(nameof(Index));

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var categoryDb = _db.Category.FirstOrDefault(u => u.Id == id);
            if (categoryDb == null)
            {
                //error
                return RedirectToAction(nameof(Index));
            }
            var categoriesForThisCategory = _db.Product.Where(u => u.category.Id == id).Count();
            if (categoriesForThisCategory > 0)
            {
                return RedirectToAction(nameof(Index));
            }
            _db.Category.Remove(categoryDb);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
