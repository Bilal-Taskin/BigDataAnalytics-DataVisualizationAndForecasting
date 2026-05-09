using BigDataOrdersDashboard.Context;
using BigDataOrdersDashboard.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BigDataOrdersDashboard.Controllers
{
    public class ReviewController : Controller
    {
        private readonly BigDataOrdersDbContext _bigDataOrdersDbContext;

        public ReviewController(BigDataOrdersDbContext bigDataOrdersDbContext)
        {
            _bigDataOrdersDbContext = bigDataOrdersDbContext;
        }

        public IActionResult ReviewList(int page = 1)
        {

            int pageSize = 12; // Sayfa başına gösterilecek ürün sayısı
            var views = _bigDataOrdersDbContext.Reviews
                .OrderBy(p => p.ReviewId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(p => p.Product)
                .Include(z => z.Customer)
                .ToList();
            int totalReviews = _bigDataOrdersDbContext.Reviews.Count();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalReviews / pageSize);

            return View(views);

        }

        [HttpGet]
        public IActionResult CreateReview()
        {
              return View();
        }


        [HttpPost]
        public IActionResult CreateReview(Review review)
        {
            review.ReviewDate = DateTime.Now;
            _bigDataOrdersDbContext.Add(review);
            _bigDataOrdersDbContext.SaveChanges();
            return RedirectToAction("ReviewList");
        }

        public IActionResult DeleteReview(int id)
        {
            var value = _bigDataOrdersDbContext.Reviews.Find(id);
            _bigDataOrdersDbContext.Remove(value);
            _bigDataOrdersDbContext.SaveChanges();
            return RedirectToAction("ReviewList");
        }

        [HttpGet]
        public IActionResult UpdateReview(int id)
        {
            var value = _bigDataOrdersDbContext.Reviews.Find(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult UpdateReview(Review Review)
        {
            var value = _bigDataOrdersDbContext.Reviews.Find(Review.ReviewId);
            value.ReviewDate = Review.ReviewDate;
            value.ReviewText = Review.ReviewText;
            value.Sentiment = Review.Sentiment;
            value.Rating = Review.Rating;
            value.PurchaseType = Review.PurchaseType;
            

            _bigDataOrdersDbContext.Update(value);
            _bigDataOrdersDbContext.SaveChanges();
            return RedirectToAction("ReviewList");
        }
    }
}

