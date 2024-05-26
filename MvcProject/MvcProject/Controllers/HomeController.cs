using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            Courses = _context.Courses.Include(x => x.CourseTags).ThenInclude(x => x.Tag).Include(x => x.Category).Take(3).ToList(),
            Sliders = _context.Sliders.OrderBy(x => x.Order).ToList(),
            Features = _context.Features.ToList(),
            Infos = _context.Infos.OrderBy(x => x.Date).ToList(),
            Events = _context.Events.Include(x => x.EventTags).ThenInclude(x => x.Tag).Include(x => x.EventTeachers).ThenInclude(x => x.Teacher).Take(8).ToList(),
            TestiMonies = _context.TestiMonies.OrderBy(x=>x.Order).ToList()

        };
        return View(homeViewModel);
    }   
}

