using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcProject.Data;
using MvcProject.Models;
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

		public IActionResult Index(int page=1)
		{
            var query = _context.Events;

            var pageData = PaginatedList<Event>.Create(query, page, 3);
            if (pageData.TotalPages < page)
            {
                return RedirectToAction("index", new { page = pageData.TotalPages });
            }
            return View(pageData);
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
        public IActionResult FilterByTag(int Id)
        {
            var eventQuery = _context.Events.Include(e => e.EventTags).ThenInclude(x => x.Tag).AsQueryable();

            if (Id != 0)
            {
                eventQuery = eventQuery.Where(x => x.EventTags.Any(w => w.TagId == Id));
            }

            var events = eventQuery.ToList();

            if (events.Count == 0)
            {
                events = _context.Events.ToList();
            }
            return View("Index", events);
        }
       
        public async Task<IActionResult> LoadMore(int skipCount)
        {
            var events = await _context.Events
                                       .OrderBy(e => e.Date)
                                       .Skip(skipCount)
                                       .Take(3)
                                       .ToListAsync();

            if (events == null || events.Count == 0)
            {
                return NoContent();
            }

            return Json(events);
        }
    }
}

