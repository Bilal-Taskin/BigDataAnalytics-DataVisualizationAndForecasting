using BigDataOrdersDashboard.Context;
using Microsoft.AspNetCore.Mvc;

namespace BigDataOrdersDashboard.ViewComponents.CustomerDetailViewComponents
{
    public class _CustomerDetailCustomerStatisticsComponentParital:ViewComponent
    {
        private readonly BigDataOrdersDbContext _context;

        public _CustomerDetailCustomerStatisticsComponentParital(BigDataOrdersDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke(int id)
        {
            id = 4;
            
            ViewBag.TotalOrderCount = _context.Orders.Where(o => o.CustomerId == id).Count();
            ViewBag.CompletedOrderCount = _context.Orders.Where(o => o.CustomerId == id && o.OrderStatus == "Tamamlandı").Count();
            ViewBag.CanceledOrderCount = _context.Orders.Where(o => o.CustomerId == id && o.OrderStatus == "İptal Edildi").Count();

            ViewBag.CustomerCountry = _context.Customers.Where(c => c.CustomerId== id).Select(c => c.CustomerCountry).FirstOrDefault();
            ViewBag.CustomerCity= _context.Customers.Where(c => c.CustomerId== id).Select(c => c.CustomerCity).FirstOrDefault();

            //ViewBag.TotalSpentAmount = _context.Orders.Where(o => o.CustomerId == id && o.OrderStatus == "Tamamlandı").Sum(o => o.); 

            return View();
        }
    }
}
