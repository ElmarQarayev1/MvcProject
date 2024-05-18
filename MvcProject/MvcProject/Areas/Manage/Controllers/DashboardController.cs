
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MvcProject.Areas.Manage.Controllers
{
	[Area("manage")]
	[Authorize]
	public class DashboardController:Controller
	{
		public IActionResult Index()
		{
			return View();
		}
		
	}
}
