using BigDataOrdersDashboard.Context;
using BigDataOrdersDashboard.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BigDataOrdersDashboard.Controllers
{
    public class CategoryController : Controller
    {
        private readonly BigDataOrdersDbContext _bigDataOrdersDbContext;

        public CategoryController(BigDataOrdersDbContext bigDataOrdersDbContext)
        {
            _bigDataOrdersDbContext = bigDataOrdersDbContext;
        }

        public IActionResult CategoryList()
        {
            var values = _bigDataOrdersDbContext.Categories.ToList();
            return View(values);
        }

        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }


        [HttpPost]
        public IActionResult CreateCategory(Category category)
        {
            
            _bigDataOrdersDbContext.Add(category);
            _bigDataOrdersDbContext.SaveChanges();
            return RedirectToAction("CategoryList");
        }

        public IActionResult DeleteCategory(int id)
        {
            var value = _bigDataOrdersDbContext.Categories.Find(id);
            _bigDataOrdersDbContext.Remove(value);
            _bigDataOrdersDbContext.SaveChanges();
            return RedirectToAction("CategoryList");
        }

        [HttpGet]
        public IActionResult UpdateCategory(int id)
        {
            var value = _bigDataOrdersDbContext.Categories.Find(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult UpdateCategory(Category category)
        {
            var value = _bigDataOrdersDbContext.Categories.Find(category.CategoryId);
            value.CategoryName = category.CategoryName;
            _bigDataOrdersDbContext.Update(value);
            _bigDataOrdersDbContext.SaveChanges();
            return RedirectToAction("CategoryList");
        }
    }
}
