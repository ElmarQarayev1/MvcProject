using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcProject.Data;
using MvcProject.ViewModels;

namespace MvcProject.Controllers
{
	public class EventController:Controller
	{
        private readonly AppDbContext _context;

        public EventController(AppDbContext context)
		{
            _context = context;
        }

		public IActionResult Index()
		{
			var Events = _context.Events.ToList();
			return View(Events);
		}
        public IActionResult Detail(int id)
        {
            var Event = _context.Events.Include(x => x.EventTeachers).ThenInclude(x=>x.Teacher).Include(x => x.EventTags).Include(x => x.Category).FirstOrDefault(x => x.Id == id);

            if (Event == null)
            {
                return RedirectToAction("notfound", "error");
            }
            EventDetailViewModel edv = new EventDetailViewModel()
            {
                Event = Event,
                Categories = _context.Categories.ToList()
            };
            return View(edv);
         }

        public IActionResult FilterByCategory(int Id)
        {
            var eventsQuery = _context.Events.Include(e => e.Category).AsQueryable();

            if (Id != 0)
            {
                eventsQuery = eventsQuery.Where(e => e.CategoryId == Id);
            }

            var events = eventsQuery.ToList();

            if (events.Count == 0) 
            {
                events = _context.Events.ToList(); 
            }

            return View("Index", events);
        }


    }
}

