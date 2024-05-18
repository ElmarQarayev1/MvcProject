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
            return View(PaginatedList<Info>.Create(query,page,2));
        }

	}
}

