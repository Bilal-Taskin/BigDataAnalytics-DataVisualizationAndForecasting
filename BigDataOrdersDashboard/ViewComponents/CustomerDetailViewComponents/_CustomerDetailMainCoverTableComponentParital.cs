using BigDataOrdersDashboard.Context;
using Microsoft.AspNetCore.Mvc;

namespace BigDataOrdersDashboard.ViewComponents.CustomerDetailViewComponents
{
    public class _CustomerDetailMainCoverTableComponentParital:ViewComponent
    {
        private readonly BigDataOrdersDbContext _context;

        public _CustomerDetailMainCoverTableComponentParital(BigDataOrdersDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke(int id)
        {
            id = 4;
            var value = _context.Customers.Where(x=>x.CustomerId == id).FirstOrDefault();
            return View(value);
        }
    }
}
