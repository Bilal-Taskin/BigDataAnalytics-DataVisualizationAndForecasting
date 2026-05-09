using BigDataOrdersDashboard.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BigDataOrdersDashboard.ViewComponents.DashboardViewComponents
{
    public class _DashboardLast5ReviewComponentPartial: ViewComponent
    {
        private readonly BigDataOrdersDbContext _context;

        public _DashboardLast5ReviewComponentPartial(BigDataOrdersDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {

            var reviews = _context.Reviews.OrderByDescending(x => x.ReviewDate).ThenBy(z=>z.Customer.CustomerName).Include(x=>x.Product).Include(z=>z.Customer).Take(5).ToList();
           
            return View(reviews);
        }
    }
}
