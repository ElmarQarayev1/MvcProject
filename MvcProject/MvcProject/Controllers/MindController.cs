using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcProject.Data;
using MvcProject.Models;

namespace MvcProject.Controllers
{
    public class MindController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public MindController(AppDbContext context,UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public  async Task<IActionResult> MindForm(Mind mind)
        {
            if (User.Identity.IsAuthenticated)
            {
                AppUser? user = await _userManager.GetUserAsync(User);

                if (user == null || !await _userManager.IsInRoleAsync(user, "member"))
                    return RedirectToAction("index", "contact", mind);

                if (_context.Minds.Any(x => x.Id == mind.Id && x.AppUserId == user.Id))
                    return RedirectToAction("notfound", "error");
                mind.AppUserId = user.Id;
            }         
            if (!ModelState.IsValid)
            {
                return RedirectToAction("index","contact",mind);
            }
            if (mind.Name == null || mind.Email == null||mind.Subject==null|| mind.Text == null)
            {
                return RedirectToAction("index", "contact");
            }
            mind.CreatedAt = DateTime.Now;
            _context.Minds.Add(mind);
            _context.SaveChanges();
            return RedirectToAction("index", "home");
        }
       
    }
}
