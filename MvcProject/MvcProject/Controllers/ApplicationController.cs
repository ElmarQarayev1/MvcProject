using System;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcProject.Data;
using MvcProject.Models;

namespace MvcProject.Controllers
{
	public class ApplicationController:Controller
	{
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ApplicationController(AppDbContext context,UserManager<AppUser> userManager)
		{
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Apply(Application app)
        {
            Course course = _context.Courses.FirstOrDefault(x => x.Id == app.CourseId);
            if (course == null)
            {
                return RedirectToAction("notfound", "error");

            }
            if (User.Identity.IsAuthenticated)
            {
                AppUser? user = await _userManager.GetUserAsync(User);
                
                if (user == null || !await _userManager.IsInRoleAsync(user, "member"))
                    return RedirectToAction("detail", "Course", app);

                if (_context.Applications.Any(x => x.Id == app.Id && x.AppUserId == user.Id))
                    return RedirectToAction("notfound", "error");  
                app.AppUser = user;
                app.CreatedAt = DateTime.Now;
                _context.Applications.Add(app);
                _context.SaveChanges();
                return RedirectToAction("index", "home");
            }
            else
            {
                if(app.FullName==null|| app.Email == null)
                {
                    return RedirectToAction("detail","course");
                }
                Application app2 = new Application()
                {
                    FullName = app.FullName,
                    Email = app.Email,
                    CourseId = app.CourseId,
                    CreatedAt=DateTime.UtcNow,

                };
               
                _context.Applications.Add(app2);
                _context.SaveChanges();
                return RedirectToAction("index", "home");

            }
        }
        [Authorize(Roles = "member")]
        public IActionResult Cancel(int id)
        {
            AppUser user = _userManager.GetUserAsync(User).Result;

            Application app = _context.Applications.FirstOrDefault(x => x.Id == id && x.AppUserId == user.Id && x.Status == Models.Enum.ApplicationStatus.Pending);

            if (app == null) return RedirectToAction("notfound", "error");

            app.Status = Models.Enum.ApplicationStatus.Canceled;
            _context.SaveChanges();
            return RedirectToAction("profile", "account");
        }
    }
}

