
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcProject.Data;

namespace MvcProject.Controllers
{
	public class TeacherController:Controller
	{
        private readonly AppDbContext _context;

        public TeacherController(AppDbContext context)
		{
            _context = context;
        }
		public IActionResult Index()
		{
			var teacher = _context.Teachers.ToList();
			return View(teacher);
		}
	}
}

