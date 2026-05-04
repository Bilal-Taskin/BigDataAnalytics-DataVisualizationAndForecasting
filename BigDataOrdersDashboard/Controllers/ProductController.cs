using BigDataOrdersDashboard.Context;
using BigDataOrdersDashboard.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BigDataOrdersDashboard.Controllers
{
    public class ProductController : Controller
    {

        private readonly BigDataOrdersDbContext _bigDataOrdersDbContext;

        public ProductController(BigDataOrdersDbContext bigDataOrdersDbContext)
        {
            _bigDataOrdersDbContext = bigDataOrdersDbContext;
        }

        public IActionResult ProductList(int page=1)
        {
            
                int pageSize = 12; // Sayfa başına gösterilecek ürün sayısı
                var views = _bigDataOrdersDbContext.Products
                    .OrderBy(p=>p.ProductId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Include(p => p.Category)
                    .ToList();
                int totalProducts = _bigDataOrdersDbContext.Products.Count();
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

                return View(views);

        }

        [HttpGet]
        public IActionResult CreateProduct()
        {
            var categoryList = _bigDataOrdersDbContext.Categories
                                .Select(selector => new SelectListItem
                                {
                                    Text = selector.CategoryName,
                                    Value = selector.CategoryId.ToString()
                                }).ToList();
            
            ViewBag.CategoryList = categoryList;
            return View();
        }


        [HttpPost]
        public IActionResult CreateProduct(Product Product)
        {

            _bigDataOrdersDbContext.Add(Product);
            _bigDataOrdersDbContext.SaveChanges();
            return RedirectToAction("ProductList");
        }

        public IActionResult DeleteProduct(int id)
        {
            var value = _bigDataOrdersDbContext.Products.Find(id);
            _bigDataOrdersDbContext.Remove(value);
            _bigDataOrdersDbContext.SaveChanges();
            return RedirectToAction("ProductList");
        }

        [HttpGet]
        public IActionResult UpdateProduct(int id)
        {
            var categoryList = _bigDataOrdersDbContext.Categories
                               .Select(selector => new SelectListItem
                               {
                                   Text = selector.CategoryName,
                                   Value = selector.CategoryId.ToString()
                               }).ToList();

            ViewBag.CategoryList = categoryList;
            var value = _bigDataOrdersDbContext.Products.Find(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult UpdateProduct(Product Product)
        {
            var value = _bigDataOrdersDbContext.Products.Find(Product.ProductId);
            value.ProductName = Product.ProductName;
            _bigDataOrdersDbContext.Update(value);
            _bigDataOrdersDbContext.SaveChanges();
            return RedirectToAction("ProductList");
        }
    }
}
