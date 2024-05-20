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
	public class TeacherController:Controller
	{
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public TeacherController(AppDbContext context,IWebHostEnvironment env)
		{
            _env = env;
            _context = context;
        }
        public IActionResult Index(int page=1)
        {
            var query = _context.Teachers.Include(x => x.SocialMedias).Include(x => x.EventTeachers).ThenInclude(x => x.Event);

            return View(PaginatedList<Teacher>.Create(query,page,2));
        }
        public IActionResult Create()
        {
            ViewBag.Socials = _context.SocialMedias.ToList();
            ViewBag.Events = _context.Events.ToList();
            return View();
        }
        [HttpPost]
        public IActionResult Create(Teacher teacher)
        {
            if (teacher.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "ImageFile is required!!");
            }
            if (!ModelState.IsValid) return View();

            teacher.Img = FileManager.Save(teacher.ImageFile, _env.WebRootPath, "uploads/teacher");
            _context.Teachers.Add(teacher);
            _context.SaveChanges();
            return RedirectToAction("index");       
        }
        public IActionResult Delete(int id)
        {
            Teacher teacher = _context.Teachers.FirstOrDefault(m => m.Id == id);

            if (teacher is null) return RedirectToAction("notfound", "error");

            string deletedFile = teacher.Img;

            _context.Teachers.Remove(teacher);

            _context.SaveChanges();
            FileManager.Delete(_env.WebRootPath, "uploads/teacher", deletedFile);

            return RedirectToAction("Index");
        }

    }
}
