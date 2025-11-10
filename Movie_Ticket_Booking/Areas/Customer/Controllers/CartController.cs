using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movie_Ticket_Booking.Models;
using Movie_Ticket_Booking.Repositories.IRepositories;
using Stripe.Checkout;
using System.Threading.Tasks;

namespace Movie_Ticket_Booking.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Promotion> _promotionRepository;
        private readonly IRepository<PromotionUsage> _promotionUsageRepository;
        private readonly IMovieRepository _movieRepository;


        public CartController(UserManager<ApplicationUser> userManager, IRepository<Cart> cartRepository, IRepository<Promotion> promotionRepository,IMovieRepository movieRepository, IRepository<PromotionUsage> promotionUsageRepository)
        {
            _movieRepository = movieRepository;
            _userManager = userManager;
            _cartRepository = cartRepository;
            _promotionRepository = promotionRepository;
            _promotionUsageRepository = promotionUsageRepository;
        }


        public async Task<IActionResult> Index(string code)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var cart = await _cartRepository.GetAsync(e => e.ApplicationUserId == user.Id, includes: [e => e.Movie, e => e.ApplicationUser]);

            if (code is not null)
            {
                var promotion = await _promotionRepository.GetOneAsync(e => e.Code == code && e.IsValid);

                if (promotion is null)
                {
                    TempData["error-notification"] = "Invalid or expired promotion code!";
                    return View(cart);
                }

                var alreadyUsed = await _promotionUsageRepository.GetOneAsync(u => u.UserId == user.Id && u.PromotionId == promotion.Id);

                if (alreadyUsed is not null)
                {
                    TempData["error-notification"] = "You have already used this promotion code!";
                    return View(cart);
                }

                var result = cart.FirstOrDefault(e => e.MovieId == promotion.MovieId);

                if (result is not null)
                {
                    result.Price -= (decimal)result.Movie.Price * (promotion.Discount / 100);
                    await _promotionUsageRepository.AddAsync(new PromotionUsage
                    {
                        PromotionId = promotion.Id,
                        UserId = user.Id,
                        UsedAt = DateTime.Now
                    });

                    await _promotionUsageRepository.CommitAsync();
                    await _cartRepository.CommitAsync();
                    TempData["success-notification"] = "Promotion code applied successfully!";

                }
                else
                {
                    TempData["error-notification"] = "This promotion code is not applicable to any movies in your cart!";
                }
            }


                return View(cart);
        }

        public async Task<IActionResult> AddToCart(int count, int movieId, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var movieInDb = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.MovieId == movieId);

            if (movieInDb is not null)
            {
                movieInDb.Count += count;
                await _cartRepository.CommitAsync(cancellationToken);

                TempData["success-notification"] = "The number of movie tickets in the shopping cart has been successfully updated.";

                return RedirectToAction("List", "Movies");
            }

            await _cartRepository.AddAsync(new()
            {
                MovieId = movieId,
                Count = count,
                ApplicationUserId = user.Id,
                Price = (decimal)(await _movieRepository.GetOneAsync(e => e.Id == movieId)!).Price
            }, cancellationToken: cancellationToken);
            await _cartRepository.CommitAsync(cancellationToken);

            TempData["success-notification"] = "Add movie tickets to cart successfully";

            return RedirectToAction("List", "Movies");
        }

        public async Task<IActionResult> IncrementMocie(int movieId, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var movie = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.MovieId == movieId);

            if (movie is null) return NotFound();

            movie.Count += 1;
            await _cartRepository.CommitAsync(cancellationToken);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DecrementMovie(int movieId, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var movie = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.MovieId == movieId);

            if (movie is null) return NotFound();

            if (movie.Count <= 1)
                _cartRepository.Delete(movie);
            else
                movie.Count -= 1;

            await _cartRepository.CommitAsync(cancellationToken);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteMovie(int movieId, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var movie = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.MovieId == movieId);

            if (movie is null) return NotFound();

            _cartRepository.Delete(movie);
            await _cartRepository.CommitAsync(cancellationToken);

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Pay()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var cart = await _cartRepository.GetAsync(e => e.ApplicationUserId == user.Id, includes: [e => e.Movie]);

            if (cart is null) return NotFound();

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/customer/checkout/success",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/customer/checkout/cancel",
            };

            foreach (var item in cart)
            {
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "egp",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Movie.Title,
                            Description = item.Movie.Description,
                        },
                        UnitAmount = (long)item.Price * 100,
                    },
                    Quantity = item.Count,
                });
            }

            var service = new SessionService();
            var session = service.Create(options);
            return Redirect(session.Url);
        }
    }
}
