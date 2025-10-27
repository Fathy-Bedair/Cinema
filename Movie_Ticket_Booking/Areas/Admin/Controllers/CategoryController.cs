using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movie_Ticket_Booking.DataAccess;
using Movie_Ticket_Booking.Models;
using Movie_Ticket_Booking.Repositories;
using Movie_Ticket_Booking.Repositories.IRepositories;

namespace Movie_Ticket_Booking.Areas.Admin.Controllers
{
    [Area(areaName: "Admin")]
    public class CategoryController : Controller
    {
        private readonly IRepository<Category> _categoryRepository;

        public CategoryController(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)

        {
            var categories = await _categoryRepository.GetAsync(tracked: false, cancellationToken: cancellationToken);

            return View(categories);
        }

        public IActionResult Create()
        {


            return View(new Category());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category, CancellationToken cancellationToken)

        {
            if (ModelState.IsValid)
            {
                await _categoryRepository.AddAsync(category, cancellationToken);
                await _categoryRepository.CommitAsync(cancellationToken);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        public async Task<IActionResult> Edit(int id , CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);

            if (category is null)
                return NotFound();

            return View(category);

        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category category, CancellationToken cancellationToken)
        {
            var existingCategory = await _categoryRepository.GetOneAsync(e => e.Id == category.Id, cancellationToken: cancellationToken);

            if (existingCategory is null)
                return NotFound();

            if (ModelState.IsValid)
            {
                existingCategory.Name = category.Name;
                existingCategory.Description = category.Description;
                await _categoryRepository.CommitAsync(cancellationToken);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);

            if (category is not null)
            { 
                _categoryRepository.Delete(category);
                await _categoryRepository.CommitAsync(cancellationToken);
            }
            return RedirectToAction(nameof(Index));
        }



    }
}
