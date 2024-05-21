using System;
using Microsoft.AspNetCore.Mvc;
using MvcProject.Data;

namespace MvcProject.Controllers
{
	public class ContactController:Controller
	{
        private readonly AppDbContext _context;

        public ContactController(AppDbContext context)
		{
            _context = context;
        }

		public IActionResult Index()
		{

			return View();
		}

	}
}

