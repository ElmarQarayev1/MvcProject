
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


            var courses = _context.Courses.Include(x => x.CourseTags).ToList();
			
			return View(courses);
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

       [HttpPost]
        public IActionResult Search(string searchTerm)
        {
           
            if (string.IsNullOrEmpty(searchTerm))
            {
                return RedirectToAction("Index");
            }        
            var courses = _context.Courses
                .Include(x => x.CourseTags)
                .Where(c => c.Name.Contains(searchTerm) || c.Desc.Contains(searchTerm))
                .ToList();
         
            CourseViewModel cv = new CourseViewModel()
            {
                Courses = courses
            };

            return View("Index", cv); 
        }
        public IActionResult FilterByCategory(int Id)
        {
            var courseQuery = _context.Courses.Include(e => e.Category).AsQueryable();

            if (Id != 0)
            {
                courseQuery = courseQuery.Where(e => e.CategoryId == Id);
            }

            var courses = courseQuery.ToList();

            if (courses.Count == 0)
            {
                courses = _context.Courses.ToList();
            }

            return View("Index", courses);
        }

    }

}


