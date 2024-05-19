using System;
using Microsoft.AspNetCore.Mvc;

namespace MvcProject.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult NotFound()
        {
            return View();
        }
    }
}

