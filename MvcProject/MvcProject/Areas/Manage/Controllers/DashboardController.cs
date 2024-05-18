
using System;
using Microsoft.AspNetCore.Mvc;

namespace MvcProject.Areas.Manage.Controllers
{
	[Area("manage")]
	public class DashboardController:Controller
	{
		public IActionResult Index()
		{
			return View();
		}
		
	}
}
