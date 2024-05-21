using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcProject.Areas.Manage.ViewModels;
using MvcProject.Data;
using MvcProject.Models;

namespace MvcProject.Areas.Manage.Controllers
{
	[Area("manage")]
	public class SettingController:Controller
	{
        private readonly AppDbContext _context;

        public SettingController(AppDbContext context)
		{
            _context = context;
        }

        public IActionResult Index(int page = 1)
        {
            var query = _context.Settings;

            var pageData = PaginatedList<Setting>.Create(query, page, 2);

            if (pageData.TotalPages < page)
            {
                return RedirectToAction("index", new { page = pageData.TotalPages });
            }
            return View(pageData);
        }
        public IActionResult Edit(string key)
        {
           Setting setting = _context.Settings.Find(key);
            if (setting == null)
            {
                return RedirectToAction("notfound", "error");
            }
            return View(setting);
        }
        [HttpPost]
        public IActionResult Edit(Setting setting)
        {
            if (!ModelState.IsValid)
            {
                return View(setting);
            }

            Setting existsetting = _context.Settings.Find(setting.Key);

            if (existsetting == null)
            {
                return RedirectToAction("notfound", "error");
            }

            existsetting.Key = setting.Key;
            existsetting.Value = setting.Value;

            _context.SaveChanges();

            return RedirectToAction("index");
        }
        public IActionResult Delete(string key)
        {
            Setting setting = _context.Settings.Find(key);

            if (setting == null) return RedirectToAction("notfound", "error");

            _context.Settings.Remove(setting);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}

