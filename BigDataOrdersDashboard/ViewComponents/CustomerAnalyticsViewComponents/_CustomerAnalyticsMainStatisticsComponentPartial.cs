using BigDataOrdersDashboard.Context;
using Microsoft.AspNetCore.Mvc;

namespace BigDataOrdersDashboard.ViewComponents.CustomerAnalyticsViewComponents
{
    public class _CustomerAnalyticsMainStatisticsComponentPartial:ViewComponent
    {
        private readonly BigDataOrdersDbContext _context;

        public _CustomerAnalyticsMainStatisticsComponentPartial(BigDataOrdersDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var totalCustomersCount = _context.Customers.Count();

            ViewBag.TotalCustomersCount = totalCustomersCount;

            var totalOrdersCount = _context.Orders.Count();
            
            var avarageOrdersPerCustomer = totalOrdersCount / totalCustomersCount;

            ViewBag.AvarageOrdersPerCustomer = avarageOrdersPerCustomer;

            var threeMonthsAgo = DateTime.Now.AddMonths(-3);

            var activeCustomerCount  = _context.Orders.Where(o => o.OrderDate >= threeMonthsAgo).Select(o => o.CustomerId).Distinct().Count();

            ViewBag.ActiveCustomerCount = activeCustomerCount;

            var sixMonthAgo = DateTime.Now.AddMonths(-6);

            var inactiveCustomerCount = _context.Customers.Count(c => !_context.Orders.Any(o => o.CustomerId == c.CustomerId && o.OrderDate >= sixMonthAgo));

            ViewBag.InactiveCustomerCount = inactiveCustomerCount;


            return View();
        }
    }
}
