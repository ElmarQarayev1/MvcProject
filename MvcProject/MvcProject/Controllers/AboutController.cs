using System;
using Microsoft.AspNetCore.Mvc;
using MvcProject.Data;
using MvcProject.ViewModels;

namespace MvcProject.Controllers
{
	public class AboutController:Controller
	{
        private readonly AppDbContext _context;

        public AboutController(AppDbContext context)
		{
            _context = context;
        }
        public IActionResult Index()
        {
            AboutViewModel aboutViewModel = new AboutViewModel()
            {
                Infos = _context.Infos.ToList(),
                Teachers = _context.Teachers.Take(4).ToList()

            };
             return View(aboutViewModel);
        }
	}
}

