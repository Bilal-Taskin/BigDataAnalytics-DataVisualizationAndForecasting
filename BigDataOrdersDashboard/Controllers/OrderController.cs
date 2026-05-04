using BigDataOrdersDashboard.Context;
using BigDataOrdersDashboard.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BigDataOrdersDashboard.Controllers
{
    public class OrderController : Controller
    {

        private readonly BigDataOrdersDbContext _bigDataOrdersDbContext;

        public OrderController(BigDataOrdersDbContext bigDataOrdersDbContext)
        {
            _bigDataOrdersDbContext = bigDataOrdersDbContext;
        }

        public IActionResult OrderList(int page = 1)
        {

            int pageSize = 12; // Sayfa başına gösterilecek ürün sayısı
            var views = _bigDataOrdersDbContext.Orders
                .OrderBy(p => p.OrderId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(p => p.Product)
                .Include(x => x.Customer)
                .ToList();
            int totalOrders = _bigDataOrdersDbContext.Orders.Count();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalOrders / pageSize);

            return View(views);

        }

        [HttpGet]
        public IActionResult CreateOrder()
        {
             return View();
        }


        [HttpPost]
        public IActionResult CreateOrder(Order Order)
        {
            Order.OrderDate = DateTime.Parse(DateTime.Now.ToShortDateString());

            _bigDataOrdersDbContext.Add(Order);
            _bigDataOrdersDbContext.SaveChanges();
            return RedirectToAction("OrderList");
        }

        public IActionResult DeleteOrder(int id)
        {
            var value = _bigDataOrdersDbContext.Orders.Find(id);
            _bigDataOrdersDbContext.Remove(value);
            _bigDataOrdersDbContext.SaveChanges();
            return RedirectToAction("OrderList");
        }

        [HttpGet]
        public IActionResult UpdateOrder(int id)
        {
             var value = _bigDataOrdersDbContext.Orders.Find(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult UpdateOrder(Order Order)
        {
            var value = _bigDataOrdersDbContext.Orders.Find(Order.OrderId);
            value.ProductId = Order.ProductId;
            value.CustomerId = Order.CustomerId;
            value.Quantity = Order.Quantity;
            value.PaymentMethod = Order.PaymentMethod;
            value.OrderStatus = Order.OrderStatus;
            value.OrderNotes = Order.OrderNotes;
            value.OrderDate = Order.OrderDate;
            _bigDataOrdersDbContext.Update(value);
            _bigDataOrdersDbContext.SaveChanges();
            return RedirectToAction("OrderList");
        }
    }
}
