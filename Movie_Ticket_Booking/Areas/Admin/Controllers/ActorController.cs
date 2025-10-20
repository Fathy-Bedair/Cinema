using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movie_Ticket_Booking.DataAccess;
using Movie_Ticket_Booking.Models;
using System.Threading.Tasks;

namespace Movie_Ticket_Booking.Areas.Admin.Controllers
{
    [Area(areaName: "Admin")]

    public class ActorController : Controller
    {
        ApplicationDbContext Context = new();

        public IActionResult Index()
        {
            var actors = Context.Actors.ToList();
            return View(actors);
        }

        public IActionResult Create()
        {

            return View(new Actor());
        }

        [HttpPost]
        public IActionResult Create(Actor actor, IFormFile photo)
        {
            if (photo is not null)
            {

                var photoName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
                var photoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\assets\\img\\photo", photoName);
                using (var stream = System.IO.File.Create(photoPath))
                {
                    photo.CopyTo(stream);
                }

                actor.ProfilePictureURL = "/assets/img/photo/" + photoName;
            }

            if (ModelState.IsValid)
            {
                Context.Actors.Add(actor);
                Context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        public IActionResult Edit(int id)
        {
            var actor = Context.Actors.FirstOrDefault(x => x.Id == id);
            if (actor is null)
            {
                return NotFound();
            }
            return View(actor);
        }
        [HttpPost]
        public IActionResult Edit(Actor actor, IFormFile? photo)
        {
            var existingActor = Context.Actors.FirstOrDefault(x => x.Id == actor.Id);
            if (existingActor is null)
                return NotFound();


            if (photo is not null)
            {
                var photoName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
                var photoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\assets\\img\\photo", photoName);
                using (var stream = System.IO.File.Create(photoPath))
                {
                    photo.CopyTo(stream);
                }

                if (existingActor.ProfilePictureURL is not null)
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\assets\\img\\photo", existingActor.ProfilePictureURL);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                existingActor.ProfilePictureURL = "/assets/img/photo/" + photoName;
            }
            if (ModelState.IsValid)
            {
                existingActor.Name = actor.Name;
                existingActor.Bio = actor.Bio;

                Context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        public IActionResult Delete(int id)
        {
            var actor = Context.Actors.FirstOrDefault(x => x.Id == id);
            if (actor is not null)
            {
                Context.Actors.Remove(actor);
                Context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
