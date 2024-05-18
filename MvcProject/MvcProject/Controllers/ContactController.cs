using System;
using Microsoft.AspNetCore.Mvc;

namespace MvcProject.Controllers
{
	public class ContactController:Controller
	{
		public ContactController()
		{
		}

		public IActionResult Index()
		{
			return View();
		}

	}
}

