using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcProject.Areas.Manage.ViewModels;
using MvcProject.Data;
using MvcProject.Models;

namespace MvcProject.Areas.Manage.Controllers
{
    [Authorize(Roles ="admin,superadmin")]
    [Area("manage")]
	public class TagController:Controller
	{
        private readonly AppDbContext _context;

        public TagController(AppDbContext context)
		{
            _context = context;
        }
        public IActionResult Index(int page = 1)
        {
            var query = _context.Tags;
            var pageData = PaginatedList<Tag>.Create(query, page, 2);
            if (pageData.TotalPages < page)
            {
                return RedirectToAction("index", new { page = pageData.TotalPages });

            }
            return View(pageData);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Tag tag)
        {

            if (!ModelState.IsValid)
            {
                return View(tag);
            }
            _context.Tags.Add(tag);
            _context.SaveChanges();
            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            Tag tag = _context.Tags.Find(id);
            if (tag == null)
            {
                return RedirectToAction("notfound", "error");
            }
            return View(tag);
        }
        [HttpPost]
        public IActionResult Edit(Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return View(tag);
            }

            Tag existTag = _context.Tags.Find(tag.Id);

            if (existTag == null)
            {
                return RedirectToAction("notfound", "error");
            }

            existTag.Name = tag.Name;

            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            Tag tag = _context.Tags.Find(id);
            if (tag == null)
            {
                return RedirectToAction("notfound", "error");
            }
            _context.Tags.Remove(tag);
            _context.SaveChanges();
            return RedirectToAction("index");

        }

    }
}

