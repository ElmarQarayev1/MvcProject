using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcProject.Data;
using MvcProject.Models.Enum;

namespace MvcProject.Areas.Manage.Controllers
{
    [Area("manage")]
	public class MindController:Controller
	{
        private readonly AppDbContext _context;

        public MindController(AppDbContext context)
		{
            _context = context;
        }
        public IActionResult Index()
        {
            var minds = _context.Minds.Include(x => x.AppUser).ToList();
            return View(minds);
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            var mind = await _context.Minds.FindAsync(id);

            if (mind == null)
            {
                return RedirectToAction("notfound", "error");
            }
            mind.Status = MindStatus.Rejected;
            _context.Update(mind);
            await _context.SaveChangesAsync();
            return RedirectToAction("index");
        }
        [HttpPost]
        public async Task<IActionResult> Accept(int id)
        {

            var mind = await _context.Minds.FindAsync(id);

            if (mind == null)
            {
                return RedirectToAction("notfound", "error");
            }

            mind.Status = MindStatus.Accepted;
            _context.Update(mind);
            await _context.SaveChangesAsync();
            return RedirectToAction("index");
        }
    }
}

