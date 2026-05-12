using BigDataOrdersDashboard.Context;
using BigDataOrdersDashboard.Dtos.ForecastDtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;

namespace BigDataOrdersDashboard.Controllers
{

    public class ForecastController : Controller
    {
        private readonly BigDataOrdersDbContext _context;
        private readonly MLContext _mlContext;

        public ForecastController(BigDataOrdersDbContext context, MLContext mlContext)
        {
            _context = context;
            _mlContext = mlContext;
        }

        public IActionResult PaymentMethodForecast()
        {
            //2025 yılı verilerinin çekilmesi
            var startDate = new DateTime(2025, 1, 1);
            var endDate = new DateTime(2025, 12, 31);

            var monthlyPAymentData = _context.Orders
                            .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                            .AsEnumerable()
                            .GroupBy(o => new
                            {
                                Month = new DateTime(o.OrderDate.Year, o.OrderDate.Month, 1),
                                o.PaymentMethod
                            })
                            .Select(g => new
                            {
                                Month = g.Key.Month,
                                PaymentMethod = g.Key.PaymentMethod,
                                OrderCount = g.Count()
                            })
                            .OrderBy(x => x.Month)
                            .ToList();

            var forecast = new List<Object>();

            //tahmin  sonuçlarını tutacak liste

            foreach (var methode in monthlyPAymentData.Select(x => x.PaymentMethod).Distinct())
            {
                var methodData = monthlyPAymentData
                    .Where(x => x.PaymentMethod == methode)
                    .Select((x, index) => new PaymentForecastData
                    {
                        PaymentMethod = x.PaymentMethod,
                        MonthIndex = index + 1,
                        OrderCount = x.OrderCount
                    }).ToList();
                var dataView = _mlContext.Data.LoadFromEnumerable(methodData);



                //forecast modeli

                var pipeline = _mlContext.Forecasting.ForecastBySsa(

                    outputColumnName: "ForecastedValues",
                    inputColumnName: nameof(PaymentForecastData.OrderCount),
                    windowSize: 4,
                    seriesLength: methodData.Count,
                    trainSize: methodData.Count,
                    horizon: 3,
                    confidenceLevel: 0.95f
                    );

                var model = pipeline.Fit(dataView);
                var engine = model.CreateTimeSeriesEngine<PaymentForecastData, PaymentForcastPrediction>(_mlContext);
                var prediction = engine.Predict();


                //2026 OCak Şubat Mart tahminleri
                for (int i = 0; i < prediction.ForecastedValues.Length; i++)
                {
                    forecast.Add(new
                    {
                        PaymentMethod = methode,
                        Month = new DateTime(2026, i + 1, 1).ToString("yyyy MMM"),
                        ForecastedOrderCount = (int)prediction.ForecastedValues[i]
                    });
                }
            }

            return View(forecast);
        }


        public IActionResult GermanyCitiesForecast()
        {
            var startDate = new DateTime(2023, 1, 1);
            var endDate = new DateTime(2025, 12, 31);

            var germanyCityData = _context.Orders
                .Include(o => o.Customer)
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate && o.Customer.CustomerCountry == "Almanya")
                .AsEnumerable()
                .GroupBy(o => new
                {
                    o.Customer.CustomerCity,
                    Year = o.OrderDate.Year,
                    Month = o.OrderDate.Month
                })
                .Select(g => new
                {
                    City = g.Key.CustomerCity,
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    DateKey = $"{g.Key.Year}-{g.Key.Month:D2}",
                    OrderCount = g.Count()


                })
                .OrderBy(xP => xP.City)
                .ThenBy(x => x.DateKey)
                .ToList();

            var forecasts = new List<object>();

            foreach (var city in germanyCityData.Select(x => x.City).Distinct())
            {
                var cityData = germanyCityData
                    .Where(x => x.City == city)
                    .Select((x, index) => new GermanCitieasForecastData
                    {
                        City = city,
                        MonthIndex = index + 1,
                        OrderCount = x.OrderCount
                    }).ToList();

                if (cityData.Count < 4)
                    continue;

                var dataView = _mlContext.Data.LoadFromEnumerable(cityData);

                var pipeline = _mlContext.Forecasting.ForecastBySsa(
                    outputColumnName: "ForecastedValues",
                    inputColumnName: nameof(GermanCitieasForecastData.OrderCount),
                    windowSize: 12,
                    seriesLength: cityData.Count,
                    trainSize: cityData.Count,
                    horizon: 12,
                    confidenceLevel: 0.95f
                    );

                var model = pipeline.Fit(dataView);
                var engine = model.CreateTimeSeriesEngine<GermanCitieasForecastData, GermanyCitiesForecastPrediction>(_mlContext);

                var prediction = engine.Predict();

                var yearlyForecast = (int)prediction.ForecastedValues.Sum();

                var year2024Count = germanyCityData
                    .Where(x => x.City == city && x.Year == 2024)
                    .Sum(x => x.OrderCount);

                var year2025Count = germanyCityData
                    .Where(x => x.City == city && x.Year == 2025)
                    .Sum(x => x.OrderCount);

                var diff = yearlyForecast - year2025Count;
                double? growthRate = year2025Count > 0
                    ? (diff / (double)year2025Count) * 100.0
                    : (double?)null;

                forecasts.Add(new
                {
                    City = city,
                    Year2024 = year2024Count,
                    Year2025 = year2025Count,
                    Year = "2026",
                    ForecastedCount = yearlyForecast,
                    DiffTo2025 = diff,
                    GrowthRate = growthRate
                });
            }

            return View(forecasts);
        }

    }
   
}


