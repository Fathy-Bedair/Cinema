using Microsoft.AspNetCore.Mvc;
using Movie_Ticket_Booking.DataAccess;
using Movie_Ticket_Booking.Models;
using Movie_Ticket_Booking.Repositories;
using Movie_Ticket_Booking.Repositories.IRepositories;
using System.Threading;

namespace Movie_Ticket_Booking.Areas.Admin.Controllers
{
    [Area(areaName: "Admin")]

    public class CinemaController : Controller
    {

        private readonly IRepository<Cinema> _cinemaRepository;

        public CinemaController(IRepository<Cinema> cinemaRepository)
        {
            _cinemaRepository = cinemaRepository;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)

        {
            var cinemas = await _cinemaRepository.GetAsync(tracked: false, cancellationToken: cancellationToken);

            return View(cinemas);
        }

        public IActionResult Create()
        {

            return View(new Cinema());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Cinema cinema, IFormFile logo, CancellationToken cancellationToken)

        {
            if (logo is not null)
            {
                var logoName = Guid.NewGuid().ToString() + Path.GetExtension(logo.FileName);
                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\assets\\img\\cinema", logoName);
                using (var stream = System.IO.File.Create(logoPath))
                {
                    logo.CopyTo(stream);
                }
                cinema.Logo = "/assets/img/cinema/" + logoName;
            }

            if (ModelState.IsValid)
            {
                await _cinemaRepository.AddAsync(cinema, cancellationToken);
                await _cinemaRepository.CommitAsync(cancellationToken);
                return RedirectToAction(nameof(Index));
            }
            return View(cinema);
        }

        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {

            var cinema = await _cinemaRepository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);


            if (cinema is null)
            {
                return NotFound();
            }
            return View(cinema);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Cinema cinema, IFormFile? logo, CancellationToken cancellationToken)
        {
            var existingCinema = await _cinemaRepository.GetOneAsync(e => e.Id == cinema.Id, cancellationToken: cancellationToken);

            if (existingCinema is null)
                return NotFound();

            if (logo is not null)
            {
                var logoName = Guid.NewGuid().ToString() + Path.GetExtension(logo.FileName);
                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\assets\\img\\cinema", logoName);
                using (var stream = System.IO.File.Create(logoPath))
                {
                    logo.CopyTo(stream);
                }

                if (existingCinema.Logo is not null)
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\assets\\img\\cinema", existingCinema.Logo);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                existingCinema.Logo = "/assets/img/cinema/" + logoName;
            }
            if (ModelState.IsValid)
            {
                existingCinema.Name = cinema.Name;
                existingCinema.Description = cinema.Description;

                await _cinemaRepository.CommitAsync(cancellationToken);
                return RedirectToAction(nameof(Index));
            }
            return View(cinema);
        }

        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var cinema = await _cinemaRepository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);

            if (cinema is not null)
            {
                _cinemaRepository.Delete(cinema);
                await _cinemaRepository.CommitAsync(cancellationToken);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
