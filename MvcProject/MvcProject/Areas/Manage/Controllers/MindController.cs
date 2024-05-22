using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcProject.Data;
using MvcProject.Models.Enum;
using MvcProject.Services;

namespace MvcProject.Areas.Manage.Controllers
{
    [Area("manage")]
    public class MindController : Controller
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public MindController(AppDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            var minds = _context.Minds.Include(x => x.AppUser).ToList();
            return View(minds);
        }

        public IActionResult Message(int id)
        {
            var mind = _context.Minds.Include(x => x.AppUser).FirstOrDefault(x => x.Id == id);
            if (mind == null)
            {
                return RedirectToAction("NotFound", "Error");
            }

            ViewData["MindId"] = id;
            return View();
        }
        [HttpPost]
        public IActionResult Message(int id, string body, string subject)
        {
            var mind = _context.Minds.Include(x => x.AppUser).FirstOrDefault(x => x.Id == id);

            if (mind == null)
            {
                return RedirectToAction("NotFound", "Error");
            }

            if (mind.AppUser.Email != null)
            {
                _emailService.Send(mind.AppUser.Email, subject, body);
            }
            else
            {
                _emailService.Send(mind.Email, subject, body);
            }
            return RedirectToAction("Index");
        }       
        // [HttpPost]
        // public async Task<IActionResult> Reject(int id)
        // {
        //     var mind = await _context.Minds.FindAsync(id);
        //     if (mind == null)
        //     {
        //         return RedirectToAction("NotFound", "Error");
        //     }
        //     mind.Status = MindStatus.Rejected;
        //     _context.Update(mind);
        //     await _context.SaveChangesAsync();
        //     return RedirectToAction("Index");
        // }

        // [HttpPost]
        // public async Task<IActionResult> Accept(int id)
        // {
        //     var mind = await _context.Minds.FindAsync(id);
        //     if (mind == null)
        //     {
        //         return RedirectToAction("NotFound", "Error");
        //     }
        //     mind.Status = MindStatus.Accepted;
        //     _context.Update(mind);
        //     await _context.SaveChangesAsync();
        //     return RedirectToAction("Index");
        // }
    }
}
