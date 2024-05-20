using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcProject.Areas.Manage.ViewModels;
using MvcProject.Data;
using MvcProject.Helpers;
using MvcProject.Models;

namespace MvcProject.Areas.Manage.Controllers
{
    [Area("manage")]
	public class EventController:Controller
	{
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public EventController(AppDbContext context, IWebHostEnvironment env)
		{
            _env = env;
            _context = context;
        }
        public IActionResult Index(int page = 1)
        {
            var query = _context.Events.Include(x => x.EventTeachers).ThenInclude(x=>x.Teacher).Include(x => x.Category).Include(x=>x.EventTags).ThenInclude(x=>x.Tag);
            return View(PaginatedList<Event>.Create(query,page,2));
        }
        public IActionResult Delete(int id)
        {
            Event Event = _context.Events.FirstOrDefault(m => m.Id == id);

            if (Event is null) return RedirectToAction("notfound", "error");

            string deletedFile = Event.Img;

            _context.Events.Remove(Event);

            _context.SaveChanges();
            FileManager.Delete(_env.WebRootPath, "uploads/event", deletedFile);

            return RedirectToAction("Index");
        }
    }
}

