using System;
using Microsoft.AspNetCore.Mvc;
using MvcProject.Data;
using MvcProject.Models;

namespace MvcProject.Controllers
{
	public class MindController:Controller
	{
        private readonly AppDbContext _context;

        public MindController(AppDbContext context)
		{
            _context = context;
        }

        public IActionResult MindForm(Mind mind)
        {
            if (!ModelState.IsValid)
            {
                return View(mind);
            }

            _context.Minds.Add(mind);
            _context.SaveChanges();
            return RedirectToAction("index", "home");
        }


	}
}

