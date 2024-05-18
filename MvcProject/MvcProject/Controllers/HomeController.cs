using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MvcProject.Data;
using MvcProject.ViewModels;

namespace MvcProject.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }
   
    public IActionResult Index()
    {
        HomeViewModel homeViewModel = new HomeViewModel()
        {
            Sliders = _context.Sliders.OrderBy(x => x.Order).ToList(),
            Features = _context.Features.ToList(),
            Infos= _context.Infos.ToList()

        };

        return View(homeViewModel);
    }
   
}

