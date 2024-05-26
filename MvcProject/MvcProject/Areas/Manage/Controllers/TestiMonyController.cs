using System;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using MvcProject.Areas.Manage.ViewModels;
using MvcProject.Data;
using MvcProject.Helpers;
using MvcProject.Models;

namespace MvcProject.Areas.Manage.Controllers
{
    [Authorize(Roles = "admin,superadmin")]
    [Area("manage")]
	public class TestiMonyController:Controller
	{
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public TestiMonyController(AppDbContext context, IWebHostEnvironment env)
		{
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page = 1)
        {
            var query = _context.TestiMonies.OrderByDescending(x => x.Id);
            var pageData = PaginatedList<TestiMony>.Create(query, page, 2);
            if (pageData.TotalPages < page) return RedirectToAction("index", new { page = pageData.TotalPages });
            return View(pageData);

        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(TestiMony testiMony)
        {
            if (testiMony.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "ImageFile is required!!");
            }
            if (!ModelState.IsValid) return View();

            if (testiMony.ImageFile != null)
            {
                if (testiMony.ImageFile.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("ImageFile", "File must be less or equal than 2MB");
                }

                if (testiMony.ImageFile.ContentType != "image/png" && testiMony.ImageFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("ImageFile", "File type must be png,jpeg or jpg");
                }
            }

            if (!ModelState.IsValid) return View();

            testiMony.ImgName = FileManager.Save(testiMony.ImageFile, _env.WebRootPath, "uploads/testimonial");
            _context.TestiMonies.Add(testiMony);
            _context.SaveChanges();
            return RedirectToAction("index");
        }
        public IActionResult Delete(int id)
        {
            TestiMony testiMony = _context.TestiMonies.FirstOrDefault(m => m.Id == id);

            if (testiMony is null) return RedirectToAction("notfound", "error");

            string deletedFile = testiMony.ImgName;

            _context.TestiMonies.Remove(testiMony);

            _context.SaveChanges();
            FileManager.Delete(_env.WebRootPath, "uploads/testimonial", deletedFile);

            return RedirectToAction("Index");
        }
        public IActionResult Edit(int id)
        {
            TestiMony testiMony = _context.TestiMonies.FirstOrDefault(x => x.Id == id);

            if (testiMony == null) return RedirectToAction("notfound", "error");

            return View(testiMony);
        }
        [HttpPost]
        public IActionResult Edit(TestiMony testiMony)
        {
            if (!ModelState.IsValid) return View();

            TestiMony existMony = _context.TestiMonies.Find(testiMony.Id);
            if (existMony == null) return RedirectToAction("notfound", "error");
            string deletedFile = null;

            if (testiMony.ImageFile != null)
            {
                if (testiMony.ImageFile.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("ImageFile", "File must be less or equal than 2MB");
                    return View();
                }

                if (testiMony.ImageFile.ContentType != "image/png" && testiMony.ImageFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("ImageFile", "File type must be png,jpeg or jpg");
                    return View();
                }

                deletedFile = testiMony.ImgName;

                testiMony.ImgName = FileManager.Save(testiMony.ImageFile, _env.WebRootPath, "uploads/testimonial");
            }
            existMony.FullName = testiMony.FullName;
            existMony.Desc = testiMony.Desc;
            existMony.Position = testiMony.Position;
            existMony.Order = testiMony.Order;  
            _context.SaveChanges();
            if (deletedFile != null)
            {
                FileManager.Delete(_env.WebRootPath, "uploads/testimonial", deletedFile);
            }
            return RedirectToAction("index");
        }

    }
}

