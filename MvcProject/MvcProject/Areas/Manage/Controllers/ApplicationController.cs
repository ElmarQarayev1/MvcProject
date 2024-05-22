using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcProject.Areas.Manage.ViewModels;
using MvcProject.Data;
using MvcProject.Models;
using MvcProject.Models.Enum;

namespace MvcProject.Areas.Manage.Controllers
{
    [Area("manage")]
	public class ApplicationController:Controller
	{
        private readonly AppDbContext _context;

        public ApplicationController(AppDbContext context)
		{
            _context = context;
        }

        public IActionResult Index(int page = 1)
        {
            var query = _context.Applications.Include(x => x.AppUser).Include(x=>x.Course).Where(x=>x.Status!=ApplicationStatus.Canceled);
            var pageData = PaginatedList<Application>.Create(query, page, 2);
            if (pageData.TotalPages < page)
            {
                return RedirectToAction("index", new { page = pageData.TotalPages });
            }
            return View(pageData);
        }
        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            var app = await _context.Applications.FindAsync(id);
            if (app == null)
            {
                return RedirectToAction("NotFound", "Error");
            }
            app.Status = ApplicationStatus.Rejected;
            _context.Update(app);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Accept(int id)
        {
            var app = await _context.Applications.FindAsync(id);
            if (app == null)
            {
                return RedirectToAction("NotFound", "Error");
            }
            app.Status = ApplicationStatus.Accepted;
            _context.Update(app);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}

