using BigDataOrdersDashboard.Context;
using BigDataOrdersDashboard.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace BigDataOrdersDashboard.Controllers
{
    public class CustomerController : Controller
    {

        private readonly BigDataOrdersDbContext _bigDataOrdersDbContext;

        public CustomerController(BigDataOrdersDbContext bigDataOrdersDbContext)
        {
            _bigDataOrdersDbContext = bigDataOrdersDbContext;
        }

        public IActionResult CustomerList(int page = 1)
        {

            int pageSize = 12; // Sayfa başına gösterilecek ürün sayısı
            var views = _bigDataOrdersDbContext.Customers
                .OrderBy(p => p.CustomerId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            int totalCustomers = _bigDataOrdersDbContext.Customers.Count();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCustomers / pageSize);

            return View(views);

        }

        [HttpGet]
        public IActionResult CreateCustomer()
        {

            return View();
        }


        [HttpPost]
        public IActionResult CreateCustomer(Customer Customer)
        {

            _bigDataOrdersDbContext.Add(Customer);
            _bigDataOrdersDbContext.SaveChanges();
            return RedirectToAction("CustomerList");
        }

        public IActionResult DeleteCustomer(int id)
        {
            var value = _bigDataOrdersDbContext.Customers.Find(id);
            _bigDataOrdersDbContext.Remove(value);
            _bigDataOrdersDbContext.SaveChanges();
            return RedirectToAction("CustomerList");
        }

        [HttpGet]
        public IActionResult UpdateCustomer(int id)
        {
            var value = _bigDataOrdersDbContext.Customers.Find(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult UpdateCustomer(Customer Customer)
        {
            var value = _bigDataOrdersDbContext.Customers.Find(Customer.CustomerId);
            value.CustomerName = Customer.CustomerName;
            value.CustomerSurname = Customer.CustomerSurname;
            value.CustomerEmail = Customer.CustomerEmail;
            value.CustomerPhone = Customer.CustomerPhone;
            value.CustomerAddress = Customer.CustomerAddress;
            value.CustomerCity = Customer.CustomerCity;
            value.CustomerCountry = Customer.CustomerCountry;
            value.CustomerDistrict = Customer.CustomerDistrict;
            value.CustomerImageUrl = Customer.CustomerImageUrl;
            
            _bigDataOrdersDbContext.Update(value);
            _bigDataOrdersDbContext.SaveChanges();
            return RedirectToAction("CustomerList");
        }
    }
}
