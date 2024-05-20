using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcProject.Areas.Manage.ViewModels;
using MvcProject.Data;
using MvcProject.Helpers;
using MvcProject.Models;

namespace MvcProject.Areas.Manage.Controllers
{
    [Area("manage")]
	public class CourseController:Controller
	{
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CourseController(AppDbContext context,IWebHostEnvironment env)
		{
            _context = context;
            _env = env;
        }

        public IActionResult Index(int page=1)
        {
            var query = _context.Courses.Include(x => x.Category).Include(x => x.CourseTags).ThenInclude(x => x.Tag);
            return View(PaginatedList<Course>.Create(query,page,2));
        }
    
        public IActionResult Delete(int id)
        {
            Course course = _context.Courses.FirstOrDefault(m => m.Id == id);

            if (course is null) return RedirectToAction("notfound", "error");

            string deletedFile = course.Img;

            _context.Courses.Remove(course);

            _context.SaveChanges();
            FileManager.Delete(_env.WebRootPath, "uploads/course", deletedFile);

            return RedirectToAction("Index");
        }
    }
}

