using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcProject.Areas.Manage.ViewModels;
using MvcProject.Data;
using MvcProject.Models;
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
        public IActionResult Index(int page=1)
        {
            var query = _context.Minds.Include(x=>x.AppUser);
            var pageData = PaginatedList<Mind>.Create(query, page, 2);
            if (pageData.TotalPages < page)
            {
                return RedirectToAction("index", new { page = pageData.TotalPages });

            }
            return View(pageData);

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

            string recipientEmail = mind.AppUser?.Email ?? mind.Email;
            if (recipientEmail == null)
            {
                return RedirectToAction("ntofound", "error");
            }
                  
               _emailService.Send(recipientEmail, subject, body);
            
            return RedirectToAction("Index");
        }       
       
    }
}
