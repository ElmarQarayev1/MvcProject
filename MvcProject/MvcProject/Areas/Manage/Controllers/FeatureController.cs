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
	public class FeatureController:Controller
	{
        private readonly AppDbContext _context;

        public FeatureController(AppDbContext context)
		{
            _context = context;
        }

        public IActionResult Index(int page = 1)
        {
            var query = _context.Features.OrderByDescending(x => x.Id);
            var pageData = PaginatedList<Feature>.Create(query, page, 2);
            if (pageData.TotalPages < page) return RedirectToAction("index", new { page = pageData.TotalPages });

            return View(pageData);

        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Feature feature)
        {

            if (!ModelState.IsValid)
            {
                return View(feature);
            }

            _context.Features.Add(feature);
            _context.SaveChanges();
            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            Feature feature = _context.Features.Find(id);
            if (feature == null)
            {
                return RedirectToAction("notfound", "error");
            }
            return View(feature);
        }
        [HttpPost]
        public IActionResult Edit(Feature feature)
        {
            if (!ModelState.IsValid)
            {
                return View(feature);
            }

            Feature existfeature = _context.Features.Find(feature.Id);

            if (existfeature == null)
            {
                return RedirectToAction("notfound", "error");
            }

            existfeature.Desc = feature.Desc;
            existfeature.Name = feature.Name;
           
            _context.SaveChanges();

            return RedirectToAction("index");
        }
        public IActionResult Delete(int id)
        {
            Feature feature = _context.Features.FirstOrDefault(m => m.Id == id);

            if (feature==null) return RedirectToAction("notfound", "error");
            
            _context.Features.Remove(feature);

            _context.SaveChanges();
            
            return RedirectToAction("Index");
        }

    }
}

