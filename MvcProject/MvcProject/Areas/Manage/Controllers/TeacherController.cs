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
            var query = _context.Teachers.Include(x => x.EventTeachers).ThenInclude(x => x.Event);

            var pageData = PaginatedList<Teacher>.Create(query, page, 2);
            if (pageData.TotalPages < page)
            {
                return RedirectToAction("index", new { page = pageData.TotalPages });

            }
            return View(pageData);

           
        }
        public IActionResult Create()
        {
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

            if (!ModelState.IsValid)
            {
                ViewBag.Events = _context.Events.ToList();
                return View(teacher);
            }
            
            foreach (var eventId in teacher.EventIds)
            {
                if (!_context.Events.Any(x => x.Id == eventId)) return RedirectToAction("notfound", "error");

                EventTeacher eventTeacher = new EventTeacher
                {
                    EventId = eventId,
                };
                teacher.EventTeachers.Add(eventTeacher);
            }

            if (teacher.ImageFile != null)
            {
                if (teacher.ImageFile.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("ImageFile", "File must be less or equal than 2MB");
                }

                if (teacher.ImageFile.ContentType != "image/png" && teacher.ImageFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("ImageFile", "File type must be png,jpeg or jpg");
                }
            }
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
        public IActionResult Edit(int id)
        {
            Teacher teacher = _context.Teachers.Include(x => x.EventTeachers).ThenInclude(x=>x.Event).FirstOrDefault(x => x.Id == id);

            if (teacher == null) return RedirectToAction("notfound", "error");    
           
            ViewBag.Events = _context.Events.ToList();
            teacher.EventIds = teacher.EventTeachers.Select(x => x.EventId).ToList();
            return View(teacher);
        }

        [HttpPost]
        public IActionResult Edit(Teacher teacher)
        {
            Teacher? existTeacher = _context.Teachers.Include(x => x.EventTeachers).FirstOrDefault(x => x.Id == teacher.Id);

            if (existTeacher == null) return RedirectToAction("notfound", "error");
            existTeacher.EventTeachers.RemoveAll(x => !teacher.EventIds.Contains(x.EventId));

            foreach (var eventId in teacher.EventIds.FindAll(x => !existTeacher.EventTeachers.Any(bt => bt.EventId == x)))
            {
                if (!_context.Events.Any(x => x.Id == eventId)) return RedirectToAction("notfound", "error");

                EventTeacher eventTeacher = new EventTeacher
                {
                    EventId = eventId,
                };
                existTeacher.EventTeachers.Add(eventTeacher);
            }

            string deletedFile = null;

            if (teacher.ImageFile != null)
            {
                if (teacher.ImageFile.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("ImageFile", "File must be less or equal than 2MB");
                    ViewBag.Events = _context.Events.ToList();
                    return View(teacher);
                }

                if (teacher.ImageFile.ContentType != "image/png" && teacher.ImageFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("ImageFile", "File type must be png, jpeg, or jpg");
                    ViewBag.Events = _context.Events.ToList();
                    return View(teacher);
                }

                deletedFile = existTeacher.Img;
                existTeacher.Img = FileManager.Save(teacher.ImageFile, _env.WebRootPath, "uploads/teacher");
            }
           
            existTeacher.FullName = teacher.FullName;
            existTeacher.Desc = teacher.Desc;
            existTeacher.Skype = teacher.Skype;
            existTeacher.Faculty = teacher.Faculty;
            existTeacher.InnovationPercent = teacher.InnovationPercent;
            existTeacher.DevelopmentPercent = teacher.DevelopmentPercent;
            existTeacher.DesignPercent = teacher.DesignPercent;
            existTeacher.CommunacationPercent = teacher.CommunacationPercent;
            existTeacher.LanguagePercent = teacher.LanguagePercent;
            existTeacher.TeamLeaderPercent = teacher.TeamLeaderPercent;
            existTeacher.Experience = teacher.Experience;
            existTeacher.InstagramUrl = teacher.InstagramUrl;
            existTeacher.PinterestUrl = teacher.PinterestUrl;
            existTeacher.LinkEdinUrl = teacher.LinkEdinUrl;
            existTeacher.VingUrl = teacher.VingUrl;
            existTeacher.Degree = teacher.Degree;
            existTeacher.Phone = teacher.Phone;
            existTeacher.Email = teacher.Email;
            _context.SaveChanges();

            if (deletedFile != null)
            {
                FileManager.Delete(_env.WebRootPath, "uploads/teacher", deletedFile);
            }

            return RedirectToAction("index");
        }

    }
}
