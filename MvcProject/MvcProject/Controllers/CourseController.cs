
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcProject.Data;
using MvcProject.Models;
using MvcProject.ViewModels;

namespace MvcProject.Controllers
{
	public class CourseController:Controller
	{
        private readonly AppDbContext _context;

        public CourseController(AppDbContext context)
		{
            _context = context;
        }

		public IActionResult Index()
		{
			CourseViewModel cv = new CourseViewModel()
			{
				Courses = _context.Courses.Include(x => x.CourseTags).ToList(),	
			};
			return View(cv);
		}

        public IActionResult Detail(int id)
        {
            var course = _context.Courses
                .Include(x => x.CourseTags)
                .Include(x => x.Category)
                .FirstOrDefault(x => x.Id == id);

            if (course == null)
            {
                return RedirectToAction("notfound", "error");
            }

            CourseDetailViewModel cdv = new CourseDetailViewModel()
            {
                Course = course,
                Tags = _context.Tags.ToList(),
                Categories = _context.Categories.ToList()
            };

            return View(cdv);
        }

    }
}

