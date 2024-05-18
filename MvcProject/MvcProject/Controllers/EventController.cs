using System;
using Microsoft.AspNetCore.Mvc;

namespace MvcProject.Controllers
{
	public class EventController:Controller
	{
		public EventController()
		{

		}

		public IActionResult Index()
		{
			return View();
		}
	}
}

