using System;
using Microsoft.AspNetCore.Mvc;

namespace MvcProject.Areas.Manage.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult NotFound()
        {
            return View();
        }
    }
}

