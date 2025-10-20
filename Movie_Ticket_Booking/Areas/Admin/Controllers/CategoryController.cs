using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movie_Ticket_Booking.DataAccess;
using Movie_Ticket_Booking.Models;

namespace Movie_Ticket_Booking.Areas.Admin.Controllers
{
    [Area(areaName: "Admin")]
    public class CategoryController : Controller
    {
        ApplicationDbContext Context = new();
        public IActionResult Index()
        {
            var categories = Context.Categories.ToList();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View(new Category());
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                Context.Categories.Add(category);
                Context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        public IActionResult Edit(int id)
        {
            var category = Context.Categories.FirstOrDefault(x => x.Id == id);

            if (category is null)
                return NotFound();

            return View(category);

        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            var existingCategory = Context.Categories.FirstOrDefault(x => x.Id == category.Id);

            if (existingCategory is null)
                return NotFound();

            if (ModelState.IsValid)
            {
                existingCategory.Name = category.Name;
                existingCategory.Description = category.Description; 
                Context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        public IActionResult Delete(int id)
        {
            var category = Context.Categories.FirstOrDefault(x => x.Id == id);
            if (category is not null)
            {
                Context.Categories.Remove(category);
                Context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }



    }
}
