using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcProject.Areas.Manage.ViewModels;
using MvcProject.Data;
using MvcProject.Helpers;
using MvcProject.Models;

namespace MvcProject.Areas.Manage.Controllers
{
    [Authorize(Roles ="admin,superadmin")]
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
            var pageData = PaginatedList<Course>.Create(query, page, 2);
            if (pageData.TotalPages < page)
            {
                return RedirectToAction("index", new { page = pageData.TotalPages });
               
            }
            return View(pageData);
        }


        public IActionResult Delete(int id)
        {
            Course course = _context.Courses.FirstOrDefault(m => m.Id == id);

            if (course is null) return RedirectToAction("notfound", "error");
            string deletedFile = course.Img;
            course.IsDeleted = true;
            course.ModifiedAt = DateTime.UtcNow;
            _context.Courses.Remove(course);
            _context.SaveChanges();
            FileManager.Delete(_env.WebRootPath, "uploads/course", deletedFile);

            return RedirectToAction("Index");
        }


        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Tags = _context.Tags.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Course course)
        {
            if (course.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "ImageFile is required!!");
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _context.Categories.ToList();
                ViewBag.Tags = _context.Tags.ToList();
                return View(course);
            }
            if (!_context.Categories.Any(x => x.Id == course.CategoryId))
              return RedirectToAction("notfound", "error");
            foreach (var tagId in course.TagIds)
            {
                if (!_context.Tags.Any(x => x.Id == tagId)) return RedirectToAction("notfound", "error");

                CourseTag courseTag = new CourseTag
                {
                    TagId = tagId,
                };
                course.CourseTags.Add(courseTag);
            }
            if (course.ImageFile != null)
            {
                if (course.ImageFile.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("ImageFile", "File must be less or equal than 2MB");
                }

                if (course.ImageFile.ContentType != "image/png" && course.ImageFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("ImageFile", "File type must be png,jpeg or jpg");
                }
            }
            course.Img = FileManager.Save(course.ImageFile, _env.WebRootPath, "uploads/course");
            course.CreatedAt = DateTime.UtcNow;

            _context.Courses.Add(course);

            _context.SaveChanges();

            return RedirectToAction("index");
        }



        public IActionResult Edit(int id)
        {
            Course course = _context.Courses.Include(x => x.CourseTags).FirstOrDefault(x => x.Id == id);

            if (course == null) return RedirectToAction("notfound", "error");

            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Tags = _context.Tags.ToList();
            course.TagIds = course.CourseTags.Select(x => x.TagId).ToList();
            return View(course);
        }
        [HttpPost]
        public IActionResult Edit(Course course)
        {
            Course? existcourse = _context.Courses.Include(x => x.CourseTags).FirstOrDefault(x => x.Id == course.Id);

            if (existcourse == null) return RedirectToAction("notfound", "error");

            if (course.CategoryId != existcourse.CategoryId && !_context.Categories.Any(x => x.Id == course.CategoryId))
                return RedirectToAction("notfound", "error");

            existcourse.CourseTags.RemoveAll(x => !course.TagIds.Contains(x.TagId));

            foreach (var tagId in course.TagIds.FindAll(x => !existcourse.CourseTags.Any(bt => bt.TagId == x)))
            {
                if (!_context.Tags.Any(x => x.Id == tagId)) return RedirectToAction("notfound", "error");

                CourseTag courseTag = new CourseTag
                {
                    TagId = tagId,
                };
                existcourse.CourseTags.Add(courseTag);
            }

            string deletedFile = null;

            if (course.ImageFile != null)
            {
                if (course.ImageFile.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("ImageFile", "File must be less or equal than 2MB");
                    ViewBag.Categories = _context.Categories.ToList();
                    ViewBag.Tags = _context.Tags.ToList();
                    return View(course);
                }

                if (course.ImageFile.ContentType != "image/png" && course.ImageFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("ImageFile", "File type must be png, jpeg, or jpg");
                    ViewBag.Categories = _context.Categories.ToList();
                    ViewBag.Tags = _context.Tags.ToList();
                    return View(course);
                }
                deletedFile = existcourse.Img;
                existcourse.Img = FileManager.Save(course.ImageFile, _env.WebRootPath, "uploads/course");
            }     
            existcourse.Name = course.Name;
            existcourse.Desc = course.Desc;
            existcourse.StartDate = course.StartDate;
            existcourse.HowtoApply = course.HowtoApply;
            existcourse.Duration = course.Duration;
            existcourse.ClassDuration = course.ClassDuration;
            existcourse.Certification = course.Certification;
            existcourse.Language = course.Language;
            existcourse.ModifiedAt = DateTime.UtcNow;

            _context.SaveChanges();

            if (deletedFile != null)
            {
                FileManager.Delete(_env.WebRootPath, "uploads/course", deletedFile);
            }

            return RedirectToAction("index");
        }

    }
}

