using Microsoft.AspNetCore.Mvc;

namespace BigDataOrdersDashboard.ViewComponents.AdminLayoutViewComponents
{
    public class _AdminLayoutPageLoaderComponentResult: ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
