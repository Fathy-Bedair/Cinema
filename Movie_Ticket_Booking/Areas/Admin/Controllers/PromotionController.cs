using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Movie_Ticket_Booking.Models;
using Movie_Ticket_Booking.Repositories.IRepositories;
using Movie_Ticket_Booking.Utitlies;
using System.Threading.Tasks;

namespace Movie_Ticket_Booking.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE_ROLE}")]
    public class PromotionController : Controller
    {

        private readonly IRepository<Promotion> _promotionRepository;
        private readonly IMovieRepository _movieRepository;

        public PromotionController(IRepository<Promotion> promotionRepository, IMovieRepository movieRepository)
        {
            _promotionRepository = promotionRepository;
            _movieRepository = movieRepository;
        }

        public async Task<IActionResult> Index()
        {

            var promotions = await _promotionRepository.GetAsync(includes: [e => e.Movie]);
            return View(promotions);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.movies = await _movieRepository.GetAsync(tracked: false);

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Promotion promotion, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.movies = await _movieRepository.GetAsync(tracked: false);
                return View(promotion);
            }

            var movieInDb = await _movieRepository.GetOneAsync(e => e.Id == promotion.MovieId);
            if (movieInDb is null)
            {
                TempData["error-notification"] = "Movie not found!";
                ViewBag.movies = await _movieRepository.GetAsync(tracked: false);
                return View(promotion);
            }

            if (promotion.PublishAt >= promotion.ValidTo)
            {
                TempData["error-notification"] = "Invalid promotion period!";
                ViewBag.movies = await _movieRepository.GetAsync(tracked: false);
                return View(promotion);
            }

            if (promotion.Discount <= 0 || promotion.Discount > 100)
            {
                TempData["error-notification"] = "Discount must be between 0 and 100!";
                ViewBag.movies = await _movieRepository.GetAsync(tracked: false);
                return View(promotion);
            }

            if (string.IsNullOrWhiteSpace(promotion.Code))
            {
                TempData["error-notification"] = "Promotion code cannot be empty!";
                ViewBag.movies = await _movieRepository.GetAsync(tracked: false);
                return View(promotion);
            }

            promotion.IsValid = DateTime.UtcNow >= promotion.PublishAt && DateTime.UtcNow <= promotion.ValidTo;

            promotion.Movie = movieInDb;
            await _promotionRepository.AddAsync(promotion);
            await _promotionRepository.CommitAsync();
            TempData["success-notification"] = "Promotion created successfully!";
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id)
        {
            var promotionInDb = await _promotionRepository.GetOneAsync(e => e.Id == id);
            if (promotionInDb is null)
            {
                TempData["error-notification"] = "Promotion not found!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.movies = await _movieRepository.GetAsync(tracked: false);
            return View(promotionInDb);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Promotion promotion, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.movies = await _movieRepository.GetAsync(tracked: false);
                return View(promotion);
            }
            var promotionInDb = await _promotionRepository.GetOneAsync(e => e.Id == promotion.Id);
            if (promotionInDb is null)
            {
                TempData["error-notification"] = "Promotion not found!";
                ViewBag.movies = await _movieRepository.GetAsync(tracked: false);
                return RedirectToAction(nameof(Index));
            }
            var movieInDb = await _movieRepository.GetOneAsync(e => e.Id == promotion.MovieId);
            if (movieInDb is null)
            {
                TempData["error-notification"] = "Movie not found!";
                ViewBag.movies = await _movieRepository.GetAsync(tracked: false);
                return View(promotion);
            }


            if (promotion.PublishAt >= promotion.ValidTo)
            {
                TempData["error-notification"] = "Invalid promotion period!";
                ViewBag.movies = await _movieRepository.GetAsync(tracked: false);
                return View(promotion);
            }

            if (promotion.Discount <= 0 || promotion.Discount > 100)
            {
                TempData["error-notification"] = "Discount must be between 0 and 100!";
                ViewBag.movies = await _movieRepository.GetAsync(tracked: false);
                return View(promotion);
            }

            if (string.IsNullOrWhiteSpace(promotion.Code))
            {
                TempData["error-notification"] = "Promotion code cannot be empty!";
                ViewBag.movies = await _movieRepository.GetAsync(tracked: false);
                return View(promotion);
            }

            promotionInDb.MovieId = promotion.MovieId;
            promotionInDb.Movie = movieInDb;
            promotionInDb.PublishAt = promotion.PublishAt;
            promotionInDb.ValidTo = promotion.ValidTo;
            promotionInDb.IsValid = promotion.IsValid;
            promotionInDb.Code = promotion.Code;
            promotionInDb.Discount = promotion.Discount;
            promotionInDb.IsValid = DateTime.UtcNow >= promotion.PublishAt && DateTime.UtcNow <= promotion.ValidTo;

            _promotionRepository.Update(promotionInDb);
            await _promotionRepository.CommitAsync();
            TempData["success-notification"] = "Promotion updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var promotionInDb = await _promotionRepository.GetOneAsync(e => e.Id == id);
            if (promotionInDb is null)
            {
                TempData["error-notification"] = "Promotion not found!";
                return RedirectToAction(nameof(Index));
            }
            _promotionRepository.Delete(promotionInDb);
            await _promotionRepository.CommitAsync();
            TempData["success-notification"] = "Promotion deleted successfully!";
            return RedirectToAction(nameof(Index));
        }



    }
}
