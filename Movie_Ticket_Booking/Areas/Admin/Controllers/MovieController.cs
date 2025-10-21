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

        public IActionResult Edit(int id)
        {
            var movie = context.Movies
                .Include(m => m.MovieSubImages)
                .Include(m => m.Actors)
                .FirstOrDefault(m => m.Id == id);

            if (movie == null)
                return NotFound();

            var movieVM = new MovieVM
            {
                Movie = movie,
                Categories = context.Categories.ToList(),
                Cinemas = context.Cinemas.ToList(),
                Actors = context.Actors.ToList()
            };

            return View(movieVM);
        }

        [HttpPost]
        public IActionResult Edit(MovieVM movieVM, IFormFile? poster, List<IFormFile>? subImgs, List<int>? Actors)
        {
            var transaction = context.Database.BeginTransaction();
            try
            {
                var movieInDb = context.Movies
                    .Include(m => m.MovieSubImages)
                    .Include(m => m.Actors)
                    .FirstOrDefault(m => m.Id == movieVM.Movie.Id);

                if (movieInDb == null)
                    return NotFound();

                movieInDb.Title = movieVM.Movie.Title;
                movieInDb.Description = movieVM.Movie.Description;
                movieInDb.CategoryId = movieVM.Movie.CategoryId;
                movieInDb.CinemaId = movieVM.Movie.CinemaId;
                movieInDb.InCinema = movieVM.Movie.InCinema;
                movieInDb.ReleaseDate = movieVM.Movie.ReleaseDate;
                movieInDb.Price = movieVM.Movie.Price;

                if (poster is not null && poster.Length > 0)
                {
                    if (movieInDb.PosterURL is not null)
                    {
                        var oldPosterPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\assets\\img\\poster",movieInDb.PosterURL);
                        if (System.IO.File.Exists(oldPosterPath))
                            System.IO.File.Delete(oldPosterPath);
                    }

                    var posterName = Guid.NewGuid().ToString() + Path.GetExtension(poster.FileName);
                    var posterPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/img/poster", posterName);

                    using (var stream = System.IO.File.Create(posterPath))
                    {
                        poster.CopyTo(stream);
                    }

                    movieInDb.PosterURL = "/assets/img/poster/" + posterName;
                }

                if (subImgs is not null && subImgs.Count > 0)
                {
                    foreach (var oldImg in movieInDb.MovieSubImages)
                    {
                        var oldImgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/img/picture", oldImg.Img);
                        if (System.IO.File.Exists(oldImgPath))
                            System.IO.File.Delete(oldImgPath);
                    }

                    context.MovieSubImages.RemoveRange(movieInDb.MovieSubImages);

                    foreach (var img in subImgs)
                    {
                        var imgName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                        var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/img/picture", imgName);

                        using (var stream = System.IO.File.Create(imgPath))
                        {
                            img.CopyTo(stream);
                        }

                        context.MovieSubImages.Add(new MovieSubImage
                        {
                            MovieId = movieInDb.Id,
                            Img = "/assets/img/picture/" + imgName
                        });
                    }
                }

                movieInDb.Actors.Clear();
                if (Actors is not null && Actors.Any())
                {
                    foreach (var actorId in Actors)
                    {
                        var actor = context.Actors.Find(actorId);
                        if (actor != null)
                        {
                            movieInDb.Actors.Add(actor);
                        }
                    }
                }

                context.SaveChanges();
                transaction.Commit();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine(ex);
                return View(movieVM);
            }
        }

        public IActionResult Delete(int id)
        {
            var movie = context.Movies.Find(id);
            if (movie == null)
                return NotFound();
            context.Movies.Remove(movie);
            context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

    }
}

