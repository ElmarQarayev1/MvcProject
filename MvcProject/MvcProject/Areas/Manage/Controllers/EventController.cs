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

   [Authorize(Roles = "admin,superadmin")]
    [Area("manage")]
	public class EventController:Controller
	{
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public EventController(AppDbContext context, IWebHostEnvironment env)
		{
            _env = env;
            _context = context;
        }


        public IActionResult Index(int page = 1)
        {
            var query = _context.Events.Include(x => x.EventTeachers).ThenInclude(x=>x.Teacher).Include(x => x.Category).Include(x=>x.EventTags).ThenInclude(x=>x.Tag);

            var pageData = PaginatedList<Event>.Create(query, page, 2);
            if (pageData.TotalPages < page)
            {
                return RedirectToAction("index", new { page = pageData.TotalPages });
            }
            return View(pageData);
           
        }


        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Tags = _context.Tags.ToList();
            ViewBag.Teachers = _context.Teachers.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Event Event)
       {
            if (Event.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "ImageFile is required!!");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _context.Categories.ToList();
                ViewBag.Tags = _context.Tags.ToList();
                ViewBag.Teachers = _context.Teachers.ToList();
                return View(Event);
            }

            var diff = DateTime.Now - Event.Date;
            if (diff.TotalDays >= 1)
            {
                ModelState.AddModelError("Date", "Date must be in the future");
                ViewBag.Categories = _context.Categories.ToList();
                ViewBag.Tags = _context.Tags.ToList();
                ViewBag.Teachers = _context.Teachers.ToList();
                return View(Event);
            }


            if (Event.StartTime >= Event.EndTime)
            {
                ModelState.AddModelError("StartTime", "Start time must be before end time");
                ModelState.AddModelError("EndTime", "End time must be after start time");
                ViewBag.Categories = _context.Categories.ToList();
                ViewBag.Tags = _context.Tags.ToList();
                ViewBag.Teachers = _context.Teachers.ToList();
                return View(Event);
            }



            if (!_context.Categories.Any(x => x.Id == Event.CategoryId))
                return RedirectToAction("notfound", "error");
            foreach (var tagId in Event.TagIds)
            {
                if (!_context.Tags.Any(x => x.Id == tagId)) return RedirectToAction("notfound", "error");

                 EventTag eventTag = new EventTag
                {
                    TagId = tagId,
                };
                Event.EventTags.Add(eventTag);
            }
            foreach (var teacherId in Event.TeacherIds)
            {
                if (!_context.Teachers.Any(x => x.Id == teacherId)) return RedirectToAction("notfound", "error");

                EventTeacher eventTeacher = new EventTeacher
                {
                    TeacherId = teacherId,
                };
                Event.EventTeachers.Add(eventTeacher);
            }

            if (Event.ImageFile != null)
            {
                if (Event.ImageFile.Length > 2 *  1024 * 1024)
                {
                    ModelState.AddModelError("ImageFile", "File must be less or equal than 2MB");
                }

                if (Event.ImageFile.ContentType != "image/png" && Event.ImageFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("ImageFile", "File type must be png,jpeg or jpg");
                }
            }
            Event.Img = FileManager.Save(Event.ImageFile, _env.WebRootPath, "uploads/event");
            Event.CreatedAt = DateTime.UtcNow;

            _context.Events.Add(Event);

            _context.SaveChanges();

           return RedirectToAction("index");
        }



        public IActionResult Delete(int id)
        {
            Event Event = _context.Events.FirstOrDefault(m => m.Id == id);

            if (Event is null) return RedirectToAction("notfound", "error");

            string deletedFile = Event.Img;

            _context.Events.Remove(Event);
            Event.IsDeleted = true;
            Event.ModifiedAt = DateTime.UtcNow;
            _context.SaveChanges();

            FileManager.Delete(_env.WebRootPath, "uploads/event", deletedFile);
     
            return RedirectToAction("Index");
        }




        public IActionResult Edit(int id)
        {
            Event eevent = _context.Events.Include(x=>x.EventTags).Include(x=>x.EventTeachers).FirstOrDefault(x => x.Id == id);

            if (eevent == null) return RedirectToAction("notfound", "error");

            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Tags = _context.Tags.ToList();
            eevent.TagIds = eevent.EventTags.Select(x => x.TagId).ToList();
            ViewBag.Teachers = _context.Teachers.ToList();
            eevent.TeacherIds = eevent.EventTeachers.Select(x => x.TeacherId).ToList();
            return View(eevent);
        }




        [HttpPost]
        public IActionResult Edit(Event Event)
        {
            Event? existsEvent = _context.Events.Include(x => x.EventTeachers).Include(x => x.EventTags).FirstOrDefault(x => x.Id == Event.Id);

            if (existsEvent == null) return RedirectToAction("notfound", "error");

            if (Event.CategoryId != existsEvent.CategoryId && !_context.Categories.Any(x => x.Id == Event.CategoryId))
                return RedirectToAction("notfound", "error");

            existsEvent.EventTags.RemoveAll(x => !Event.TagIds.Contains(x.TagId));

            foreach (var tagId in Event.TagIds.FindAll(x => !existsEvent.EventTags.Any(bt => bt.TagId == x)))
            {
                if (!_context.Tags.Any(x => x.Id == tagId)) return RedirectToAction("notfound", "error");

                EventTag eventTag = new EventTag
                {
                    TagId = tagId,
                };
                existsEvent.EventTags.Add(eventTag);
            }
                string deletedFile = null;

                if (Event.ImageFile != null)
                {
                    if (Event.ImageFile.Length > 2 * 1024 * 1024)
                    {
                        ModelState.AddModelError("ImageFile", "File must be less or equal than 2MB");
                        return View();
                    }

                    if (Event.ImageFile.ContentType != "image/png" && Event.ImageFile.ContentType != "image/jpeg")
                    {
                        ModelState.AddModelError("ImageFile", "File type must be png,jpeg or jpg");
                        return View();
                    }

                    deletedFile = existsEvent.Img;

                    existsEvent.Img = FileManager.Save(Event.ImageFile, _env.WebRootPath, "uploads/event");
                }

            var diff = DateTime.Now - Event.Date;
            if (diff.TotalDays >= 1)
            {
                ModelState.AddModelError("Date", "Date must be in the future");
                ViewBag.Categories = _context.Categories.ToList();
                ViewBag.Tags = _context.Tags.ToList();
                ViewBag.Teachers = _context.Teachers.ToList();
                return View(Event);
            }


            if (Event.StartTime >= Event.EndTime)
            {
                ModelState.AddModelError("StartTime", "Start time must be before end time");
                ModelState.AddModelError("EndTime", "End time must be after start time");
                ViewBag.Categories = _context.Categories.ToList();
                ViewBag.Tags = _context.Tags.ToList();
                ViewBag.Teachers = _context.Teachers.ToList();
                return View(Event);
            }

            existsEvent.Name = Event.Name;
           existsEvent.Desc = Event.Desc;
            existsEvent.Date = Event.Date;
           existsEvent.StartTime = Event.StartTime;
           existsEvent.EndTime = Event.EndTime;
           existsEvent.CategoryId = Event.CategoryId;
           existsEvent.Venue = Event.Venue;
           existsEvent.ModifiedAt = DateTime.UtcNow;

        _context.SaveChanges();

                    if (deletedFile != null)
                    {
                        FileManager.Delete(_env.WebRootPath, "uploads/event", deletedFile);
                    }

                    return RedirectToAction("index");
                
            }

        }
    }


