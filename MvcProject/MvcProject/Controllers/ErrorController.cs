using System;
using Microsoft.AspNetCore.Mvc;

namespace MvcProject.Controllers
{
    [Area("manage")]
    public class ErrorController : Controller
    {
        public IActionResult NotFound()
        {
            return View();
        }
    }
}

