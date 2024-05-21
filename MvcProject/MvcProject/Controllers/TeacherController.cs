
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcProject.Data;
using MvcProject.ViewModels;

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

        public IActionResult Detail(int id)
        {
            var teacher = _context.Teachers
                .Include(x => x.EventTeachers)
                .ThenInclude(x=>x.Event)
                .FirstOrDefault(x => x.Id == id);

            if (teacher == null)
            {
                return RedirectToAction("notfound", "error");
            }

            
            return View(teacher);
        }

    }
}

