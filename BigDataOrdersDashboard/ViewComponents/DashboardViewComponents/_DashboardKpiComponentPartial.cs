using BigDataOrdersDashboard.Context;
using Microsoft.AspNetCore.Mvc;

namespace BigDataOrdersDashboard.ViewComponents.DashboardViewComponents
{
    public class _DashboardKpiComponentPartial : ViewComponent
    {
        private readonly BigDataOrdersDbContext _context;

        public _DashboardKpiComponentPartial(BigDataOrdersDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            #region KPI1
            var today = DateTime.Today;
            var yesterday = today.AddDays(-1);

            var todayOrderCount = _context.Orders.Where(o => o.OrderDate == today).Count();
            var yesterdayOrderCount = _context.Orders.Where(o => o.OrderDate == yesterday).Count();




            if (todayOrderCount > yesterdayOrderCount)
            {
                ViewBag.TrendingIcon = "zmdi zmdi-trending-up float-right";

            }
            else if (todayOrderCount == yesterdayOrderCount)
            {
                ViewBag.TrendingIcon = " zmdi-trending-flat float-right";
            }
            else
            {
                ViewBag.TrendingIcon = "zmdi zmdi-trending-down float-right";
            }




            decimal changeRate = 0;
            changeRate = (((decimal)(todayOrderCount - yesterdayOrderCount)) / yesterdayOrderCount) * 100;


            if (changeRate > 0)
            {
                ViewBag.ChangeRateColor = "green";
            }
            else
            {
                ViewBag.ChangeRateColor = "red";
            }

            var dailyAverageOrders = _context.Orders.GroupBy(x => x.OrderDate).Select(g => g.Count()).Average();
            double ratio = 0;
            ratio = (todayOrderCount / dailyAverageOrders) * 100.0;


            ViewBag.DailyOrderChange = Math.Round(changeRate, 2);
            ViewBag.TodayOrderCount = todayOrderCount;
            ViewBag.yesterdayOrderCount = yesterdayOrderCount;
            ViewBag.todayVsAverageRatio = Math.Round(ratio, 2);
            #endregion

            #region KPI2

            var sevenDaysAgo = today.AddDays(-7);

            var totalOrders7Days = _context.Orders.Count(x => x.OrderDate >= sevenDaysAgo && x.OrderDate < today.AddDays(1));

            var cancelledOrders7Days = _context.Orders.Count(x => x.OrderStatus == "İptal Edildi" && x.OrderDate >= sevenDaysAgo && x.OrderDate < today.AddDays(1));


            decimal cancelRate = 0;
            cancelRate = ((decimal)cancelledOrders7Days / totalOrders7Days) * 100;

            ViewBag.CancelledOrders7Days = cancelledOrders7Days;
            ViewBag.CancelRate = Math.Round(cancelRate, 2);
            ViewBag.CancelColor = "red";
            ViewBag.CancelText = cancelRate > 5 ? "Yüksek İptal Oranı ⚠️" : "Normal Düzeyde";


            #endregion

            #region KPI
            var completedOrders = _context.Orders.Count(x=>x.OrderStatus=="Tamamlandı");
            decimal completionRate = 0;
            completionRate = ((decimal)completedOrders / totalOrders7Days) * 100;
            ViewBag.CompletionRate = Math.Round(completionRate, 2);
            ViewBag.CompletedOrders = completedOrders;
            ViewBag.CompletionText = completionRate >= 80 ? "Mükemmel Performans 💪" : "İyileşme Devam Ediyor 📈";

            #endregion

            return View();
        }
    }
}
