using System;
using Microsoft.AspNetCore.Mvc;
using MvcProject.Areas.Manage.ViewModels;
using MvcProject.Data;
using MvcProject.Models;

namespace MvcProject.Areas.Manage.Controllers
{
    [Area("manage")]
	public class InfoController:Controller
	{
        private readonly AppDbContext _context;

        public InfoController(AppDbContext context)
		{
            _context = context;
        }

        public IActionResult Index(int page=1)
        {
            var query = _context.Infos;
            var pageData = PaginatedList<Info>.Create(query, page, 2);
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
        public IActionResult Create(Info info)
        {

            if (!ModelState.IsValid)
            {
                return View(info);
            }

            _context.Infos.Add(info);
            _context.SaveChanges();
            return RedirectToAction("index");
        }
        public IActionResult Edit(int id)
        {
            Info info = _context.Infos.Find(id);
            if (info == null)
            {
                return RedirectToAction("notfound", "error");
            }
            return View(info);
        }
        [HttpPost]
        public IActionResult Edit(Info info)
        {
            if (!ModelState.IsValid)
            {
                return View(info);
            }

            Info existInfo = _context.Infos.Find(info.Id);

            if (existInfo == null)
            {
                return RedirectToAction("notfound", "error");
            }

            existInfo.Date = info.Date;
            existInfo.Desc = info.Desc;

            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            Info info = _context.Infos.Find(id);
            if (info == null)
            {
                return RedirectToAction("notfound", "error");
            }
            _context.Infos.Remove(info);
            _context.SaveChanges();

            return RedirectToAction("index");


        }
	}
}

