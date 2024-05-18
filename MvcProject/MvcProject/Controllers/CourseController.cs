
using System;
using Microsoft.AspNetCore.Mvc;

namespace MvcProject.Controllers
{
	public class CourseController:Controller
	{
		public CourseController()
		{
		}

		public IActionResult Index()
		{
			return View();
		}
	}
}

