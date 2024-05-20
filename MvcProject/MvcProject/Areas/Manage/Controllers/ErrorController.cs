using System;
using Microsoft.AspNetCore.Mvc;
using MvcProject.Areas.Manage.ViewModels;

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

