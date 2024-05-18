
using System;
using Microsoft.AspNetCore.Mvc;

namespace MvcProject.Controllers
{
	public class TeacherController:Controller
	{
		public TeacherController()
		{
		}
		public IActionResult Index()
		{
			return View();
		}
	}
}

