using BigDataOrdersDashboard.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BigDataOrdersDashboard.ViewComponents.DashboardViewComponents
{
    public class _DashboardLowProductsComponentPartial:ViewComponent
    {
        private readonly BigDataOrdersDbContext _context;

        public _DashboardLowProductsComponentPartial(BigDataOrdersDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var lastAddedOrders = _context.Products.Include(x=>x.Category).Where(y=>y.StockQuantity <= 13).OrderBy(y=>y.StockQuantity).Take(30).ToList();
            return View(lastAddedOrders);
        }
    }
}
