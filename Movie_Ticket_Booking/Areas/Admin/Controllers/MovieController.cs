using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Movie_Ticket_Booking.DataAccess;
using Movie_Ticket_Booking.Models;
using Movie_Ticket_Booking.Repositories;
using Movie_Ticket_Booking.Repositories.IRepositories;
using Movie_Ticket_Booking.Utitlies;
using Movie_Ticket_Booking.ViewModels;
using System.Threading;
using System.Threading.Tasks;

namespace Movie_Ticket_Booking.Areas.Admin.Controllers
{
    [Area(areaName: "Admin")]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE_ROLE}")]

    public class MovieController : Controller
    {
        ApplicationDbContext context;
        private readonly IMovieRepository _movieRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Cinema> _cinemaRepository;
        private readonly IRepository<Actor> _actorRepository;
        private readonly IMovieSubImageRepository _movieSubImageRepository;
        private readonly IRepository<MovieActor> _movieActorsRepository;

        public MovieController(ApplicationDbContext _context, IMovieRepository movieRepository, IRepository<Category> categoryRepository, IRepository<Cinema> cinemaRepository, IRepository<Actor> actorRepository, IMovieSubImageRepository movieSubImageRepository, IRepository<MovieActor> movieActorsRepository)
        {
            context = _context;
            _movieRepository = movieRepository;
            _categoryRepository = categoryRepository;
            _cinemaRepository = cinemaRepository;
            _actorRepository = actorRepository;
            _movieSubImageRepository = movieSubImageRepository;
            _movieActorsRepository = movieActorsRepository;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var movies = await _movieRepository.GetAsync(includes: [e => e.Category, e => e.Cinema], tracked: false, cancellationToken: cancellationToken); ;
            return View(movies);
        }

        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var categories = await _categoryRepository.GetAsync(cancellationToken: cancellationToken);
            var cinemas = await _cinemaRepository.GetAsync(cancellationToken: cancellationToken);
            var actors = await _actorRepository.GetAsync(cancellationToken: cancellationToken);

            var movieVM = new MovieVM
            {
                Movie = new Movie(),
                Categories = categories.ToList(),
                Cinemas = cinemas.ToList(),
                Actors = actors.ToList()

            };
            return View(movieVM);
        }
        [HttpPost]
        public async Task<IActionResult> Create(MovieVM movieVM, IFormFile poster, List<IFormFile>? subImgs, List<int> Actors, CancellationToken cancellationToken)
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

                await _movieRepository.AddAsync(movieVM.Movie, cancellationToken);
                await _movieRepository.CommitAsync(cancellationToken);

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

                        await _movieSubImageRepository.AddAsync(
                            new MovieSubImage
                            {
                                MovieId = movieVM.Movie.Id,
                                Img = "/assets/img/picture/" + imgName,
                            },
                            cancellationToken
                        );
                    }

                    await _movieSubImageRepository.CommitAsync(cancellationToken);

                }


                if (Actors.Any())
                {
                    foreach (var actorId in Actors)
                    {
                        await _movieActorsRepository.AddAsync(new()
                        {
                            MovieId = movieVM.Movie.Id,
                            ActorId = actorId,
                        });
                    }
                    await _movieActorsRepository.CommitAsync(cancellationToken);
                }

                transaction.Commit();
                await _movieRepository.CommitAsync(cancellationToken);

            }

            catch (Exception ex)
            {

                transaction.Rollback();
                Console.WriteLine(ex);


            }
            return RedirectToAction(nameof(Index));



        }
        
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var movie = await _movieRepository.GetOneAsync(e => e.Id == id, includes: [m => m.MovieSubImages, m => m.Actors], tracked: false, cancellationToken: cancellationToken);
            if (movie == null)
                return NotFound();

            var categories = await _categoryRepository.GetAsync(cancellationToken: cancellationToken);
            var cinemas = await _cinemaRepository.GetAsync(cancellationToken: cancellationToken);
            var actors = await _actorRepository.GetAsync(cancellationToken: cancellationToken);

            var movieActors = await _movieActorsRepository.GetAsync( e => e.MovieId == id,cancellationToken: cancellationToken);


            var movieVM = new MovieVM
            {
                Movie = movie,
                Categories = categories.ToList(),
                Cinemas = cinemas.ToList(),
                Actors = actors.ToList(),
                MovieActors = movieActors.ToList()
            };

            return View(movieVM);
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Edit(MovieVM movieVM, IFormFile? poster, List<IFormFile>? subImgs, List<int>? Actors, CancellationToken cancellationToken)
        {
            var transaction = context.Database.BeginTransaction();
            try
            {

                var movieInDb = await _movieRepository.GetOneAsync(
                    e => e.Id == movieVM.Movie.Id,
                    includes: [m => m.MovieSubImages, m => m.Actors],
                    tracked: true,
                    cancellationToken: cancellationToken
                );

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
                        var oldPosterPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", movieInDb.PosterURL.TrimStart('/'));
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

                await _movieRepository.CommitAsync(cancellationToken);

                if (subImgs is not null && subImgs.Count > 0)
                {

                    foreach (var oldImg in movieInDb.MovieSubImages.ToList())
                    {
                        var oldImgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", oldImg.Img.TrimStart('/'));
                        if (System.IO.File.Exists(oldImgPath))
                            System.IO.File.Delete(oldImgPath);

                        _movieSubImageRepository.Delete(oldImg);
                    }
                    //foreach (var oldImg in movieInDb.MovieSubImages)
                    //{
                    //     var oldImgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/img/picture", oldImg.Img);
                    //     if (System.IO.File.Exists(oldImgPath))
                    //        System.IO.File.Delete(oldImgPath);
                    //}


                    //_movieSubImageRepository.RemoveRange(movieInDb.MovieSubImages);
                    await _movieSubImageRepository.CommitAsync(cancellationToken);


                    foreach (var img in subImgs)
                    {
                        var imgName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                        var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\assets\\img\\picture", imgName);

                        using (var stream = System.IO.File.Create(imgPath))
                        {
                            img.CopyTo(stream);
                        }

                        await _movieSubImageRepository.AddAsync(new MovieSubImage
                        {
                            MovieId = movieInDb.Id,
                            Img = "/assets/img/picture/" + imgName
                        }, cancellationToken);
                    }
                        await _movieSubImageRepository.CommitAsync(cancellationToken);

                }

                if (Actors is not null && Actors.Any())
                {
                    var existingMovieActors = await _movieActorsRepository.GetAsync(e => e.MovieId == movieInDb.Id,cancellationToken: cancellationToken);
                    foreach (var movieActor in existingMovieActors)
                    {
                        _movieActorsRepository.Delete(movieActor);
                    }
                    await _movieActorsRepository.CommitAsync(cancellationToken);

                    foreach (var actorId in Actors)
                    {
                        await _movieActorsRepository.AddAsync(new MovieActor
                        {
                            MovieId = movieInDb.Id,
                            ActorId = actorId,
                        }, cancellationToken);
                    }

                    await _movieActorsRepository.CommitAsync(cancellationToken);

                }

                await _movieRepository.CommitAsync(cancellationToken);
                transaction.Commit();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine("Error: " + ex.InnerException?.Message ?? ex.Message);
                ModelState.AddModelError("", ex.InnerException?.Message ?? ex.Message);
                return View(movieVM);
            }
        }
        
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _movieRepository.GetOneAsync(e => e.Id == id);
            if (movie == null)
                return NotFound();
            _movieRepository.Delete(movie);
            await _movieRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}

