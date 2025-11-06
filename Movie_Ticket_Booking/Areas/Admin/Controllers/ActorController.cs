using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movie_Ticket_Booking.DataAccess;
using Movie_Ticket_Booking.Models;
using Movie_Ticket_Booking.Repositories;
using Movie_Ticket_Booking.Repositories.IRepositories;
using Movie_Ticket_Booking.Utitlies;
using System.Threading;
using System.Threading.Tasks;

namespace Movie_Ticket_Booking.Areas.Admin.Controllers
{
    [Area(areaName: "Admin")]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE_ROLE}")]


    public class ActorController : Controller
    {
        private readonly IRepository<Actor> _actorRepository;

        public ActorController(IRepository<Actor> actorRepository)
        {
            _actorRepository = actorRepository;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var actors = await _actorRepository.GetAsync(tracked: false, cancellationToken: cancellationToken);

            return View(actors);
        }

        public IActionResult Create()
        {

            return View(new Actor());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Actor actor, IFormFile photo, CancellationToken cancellationToken)
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
                await _actorRepository.AddAsync(actor, cancellationToken);
                await _actorRepository.CommitAsync(cancellationToken);
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var actor = await _actorRepository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);

            if (actor is null)
            {
                return NotFound();
            }
            return View(actor);
        }
        [HttpPost]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Edit(Actor actor, IFormFile? photo, CancellationToken cancellationToken)
        {
            var existingActor = await _actorRepository.GetOneAsync(e => e.Id == actor.Id, cancellationToken: cancellationToken);

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

                await _actorRepository.CommitAsync(cancellationToken);
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]

        public async Task<IActionResult> Delete(int id,CancellationToken cancellationToken)
        {
            var actor = await _actorRepository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);

            if (actor is not null)
            {
                _actorRepository.Delete(actor);
                await _actorRepository.CommitAsync(cancellationToken);
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
