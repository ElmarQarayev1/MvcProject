
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
		public IActionResult Index(int page=1)
		{
            
            var query = _context.Courses.Include(x => x.CourseTags);

            var pageData = PaginatedList<Course>.Create(query, page, 2);
            if (pageData.TotalPages < page)
            {
                return RedirectToAction("index", new { page = pageData.TotalPages });
            }
            return View(pageData);
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
                Categories = _context.Categories.ToList(),
                Application = new Application()
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
            return View("Index", courses);
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
        public IActionResult FilterByTag(int Id)
        {
            var courseQuery = _context.Courses.Include(e => e.CourseTags).ThenInclude(x => x.Tag).AsQueryable();

            if (Id != 0)
            {
                courseQuery = courseQuery.Where(x=>x.CourseTags.Any(w=>w.TagId==Id));
            }

            var courses = courseQuery.ToList();

            if (courses.Count == 0)
            {
                courses = _context.Courses.ToList();
            }
            return View("Index", courses);
        }
        public List<Course> SearchLayout(string s)
        {
            var course = _context.Courses.Where(x => x.Name.Contains(s)).Take(2).ToList();

            return course;
        }

    }

}


