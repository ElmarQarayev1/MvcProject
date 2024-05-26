using System;
using Microsoft.AspNetCore.Mvc;
using MvcProject.Areas.Manage.ViewModels;
using MvcProject.Data;
using MvcProject.Models;

namespace MvcProject.Areas.Manage.Controllers
{
    [Area("manage")]
	public class CategoryController:Controller
	{
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
		{
           _context = context;
        }

        public IActionResult Index(int page = 1)
        {
            var query = _context.Categories;
            var pageData = PaginatedList<Category>.Create(query, page, 2);
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
        public IActionResult Create(Category category)
        {

            if (!ModelState.IsValid)
            {
                return View(category);
            }
            _context.Categories.Add(category);
            _context.SaveChanges();
            return RedirectToAction("index");
        }
        public IActionResult Edit(int id)
        {
            Category category = _context.Categories.Find(id);
            if (category == null)
            {
                return RedirectToAction("notfound", "error");
            }
            return View(category);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            Category existcate = _context.Categories.Find(category.Id);

            if (existcate == null)
            {
                return RedirectToAction("notfound", "error");
            }

            existcate.Name = category.Name;
         
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            Category category = _context.Categories.Find(id);
            if (category == null)
            {
                return RedirectToAction("notfound", "error");
            }
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return RedirectToAction("index");


        }

    }
}

