using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Movie_Ticket_Booking.DataAccess;
using Movie_Ticket_Booking.Models;
using Movie_Ticket_Booking.ViewModels;

namespace Movie_Ticket_Booking.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class MoviesController : Controller
    {
        ApplicationDbContext context;

        public MoviesController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index(FilterMovieVM filterMovie)
        {
            var movies = context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .AsQueryable();

            if (filterMovie.search is not null)
                movies = movies.Where(m => m.Title.Contains(filterMovie.search));

            if (filterMovie.categoryId is not null)
                movies = movies.Where(m => m.CategoryId == filterMovie.categoryId.Value);

            if (filterMovie.cinemaId is not null)
                movies = movies.Where(m => m.CinemaId == filterMovie.cinemaId.Value);

            if (filterMovie.inCinema == true)
                movies = movies.Where(m => m.InCinema);

            

            ViewBag.Categories = context.Categories.ToList();
            ViewBag.Cinemas = context.Cinemas.ToList();
            ViewBag.Search = filterMovie.search;

            return View(movies.ToList());
        }

        public IActionResult Details(int id)
        {
            var movie = context.Movies
                .Include(m => m.Cinema)
                .Include(m => m.Category)
                .Include(m => m.Actors)
                .Include(m => m.MovieSubImages)
                .FirstOrDefault(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }


            return View(movie);
        }
    }
}


