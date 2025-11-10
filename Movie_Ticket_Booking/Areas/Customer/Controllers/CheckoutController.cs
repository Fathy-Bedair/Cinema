using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Movie_Ticket_Booking.Models;
using Movie_Ticket_Booking.Repositories.IRepositories;
using Movie_Ticket_Booking.ViewModels;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Movie_Ticket_Booking.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CheckoutController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Cart> _cartRepository;

        public CheckoutController(UserManager<ApplicationUser> userManager, IRepository<Cart> cartRepository)
        {
            _userManager = userManager;
            _cartRepository = cartRepository;
        }
        public async Task<IActionResult> Success(CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var cart = await _cartRepository.GetAsync(e => e.ApplicationUserId == user.Id, includes: [e => e.Movie, e => e.ApplicationUser]);

            foreach (var movie in cart)
            {
                _cartRepository.Delete(movie);
            }
            await _cartRepository.CommitAsync(cancellationToken);

            return View();
        }

        public IActionResult Cancel()
        {
            return View();
        }
    }
}
