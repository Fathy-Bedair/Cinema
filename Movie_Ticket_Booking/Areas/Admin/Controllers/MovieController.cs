using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Movie_Ticket_Booking.DataAccess;
using Movie_Ticket_Booking.Models;
using Movie_Ticket_Booking.ViewModels;

namespace Movie_Ticket_Booking.Areas.Admin.Controllers
{
    [Area(areaName: "Admin")]
    public class MovieController : Controller
    {
        ApplicationDbContext context = new();
        public IActionResult Index()
        {
            var movies = context.Movies.Include(m => m.Category).Include(m => m.Cinema).ToList();
            return View(movies);
        }

        public IActionResult Create()
        {
            var movieVM = new MovieVM
            {
                Movie = new Movie(),
                Categories = context.Categories.ToList(),
                Cinemas = context.Cinemas.ToList(),
                Actors = context.Actors.ToList()

            };
            return View(movieVM);
        }
        [HttpPost]
        public IActionResult Create(MovieVM movieVM, IFormFile poster, List<IFormFile>? subImgs, List<int> Actors)
        {
            var transaction = context.Database.BeginTransaction();

            try
            {
                if (poster is not null && poster.Length > 0)
                {

                    var posterName = Guid.NewGuid().ToString() + Path.GetExtension(poster.FileName);
                    var posterPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\assets\\img\\poster", posterName);
                    using (var stream = System.IO.File.Create(posterPath))
                    {
                        poster.CopyTo(stream);
                    }

                    movieVM.Movie.PosterURL = "/assets/img/poster/" + posterName;

                }
                context.Movies.Add(movieVM.Movie);
                context.SaveChanges();

                if (subImgs is not null && subImgs.Count > 0)
                {
                    foreach (var img in subImgs)
                    {
                        var imgName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                        var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\assets\\img\\picture", imgName);
                        using (var stream = System.IO.File.Create(imgPath))
                        {
                            img.CopyTo(stream);
                        }
                        context.MovieSubImages.Add(new()
                        {
                            MovieId = movieVM.Movie.Id,
                            Img = "/assets/img/picture/" + imgName

                        });
                    }
                    context.SaveChanges();

                }


                if (Actors.Any())
                {
                    foreach (var actorId in Actors)
                    {
                        context.MovieActors.Add(new()
                        {
                            MovieId = movieVM.Movie.Id,
                            ActorId = actorId
                        });
                    }
                    context.SaveChanges();
                }
                transaction.Commit();
                context.SaveChanges();

            }

            catch (Exception ex)
            {

                transaction.Rollback();
                Console.WriteLine(ex);


            }
            return RedirectToAction(nameof(Index));



        }


    }
}

