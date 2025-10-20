using Microsoft.AspNetCore.Mvc;
using Movie_Ticket_Booking.DataAccess;
using Movie_Ticket_Booking.Models;

namespace Movie_Ticket_Booking.Areas.Admin.Controllers
{
    [Area(areaName: "Admin")]

    public class CinemaController : Controller
    {
        ApplicationDbContext Context = new();

        public IActionResult Index()
        {
            var cinemas = Context.Cinemas.ToList();
            return View(cinemas);
        }

        public IActionResult Create()
        {
            return View(new Cinema());
        }

        [HttpPost]
        public IActionResult Create(Cinema cinema, IFormFile logo)
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
                Context.Cinemas.Add(cinema);
                Context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(cinema);
        }

        public IActionResult Edit(int id)
        {
            var cinema = Context.Cinemas.FirstOrDefault(x => x.Id == id);
            if (cinema is null)
            {
                return NotFound();
            }
            return View(cinema);
        }

        [HttpPost]
        public IActionResult Edit(Cinema cinema, IFormFile? logo)
        {
            var existingCinema = Context.Cinemas.FirstOrDefault(x => x.Id == cinema.Id);
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

                Context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(cinema);
        }

        public IActionResult Delete(int id)
        {
            var cinema = Context.Cinemas.FirstOrDefault(x => x.Id == id);
            if (cinema is not null)
            {
                Context.Cinemas.Remove(cinema);
                Context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
