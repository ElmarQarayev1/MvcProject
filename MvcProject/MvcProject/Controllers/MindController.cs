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
            AppUser? user = await _userManager.GetUserAsync(User);
            
            if (user == null || !await _userManager.IsInRoleAsync(user, "member"))
                return RedirectToAction("index", "contact",mind);

            if (_context.Minds.Any(x => x.Id == mind.Id && x.AppUserId == user.Id))
                return RedirectToAction("notfound", "error");

            if (!ModelState.IsValid)
            {
                return RedirectToAction("index","contact",mind);
            }
            mind.AppUserId = user.Id;
            mind.CreatedAt = DateTime.Now;
            _context.Minds.Add(mind);
            _context.SaveChanges();
            return RedirectToAction("index", "home");
        }
        //[HttpPost]
        //public async Task<IActionResult> Review(BookReview review)
        //{
        //    AppUser? user = await _userManager.GetUserAsync(User);
        //    if (user == null || !await _userManager.IsInRoleAsync(user, "member"))
        //        return RedirectToAction("login", "account", new { returnUrl = Url.Action("detail", "book", new { id = review.BookId }) });

        //    if (!_context.Books.Any(x => x.Id == review.BookId && !x.IsDeleted))
        //        return RedirectToAction("notfound", "error");

        //    if (_context.BookReviews.Any(x => x.Id == review.BookId && x.AppUserId == user.Id))
        //        return RedirectToAction("notfound", "error");


        //    if (!ModelState.IsValid)
        //    {
        //        var vm = await getBookDetail(review.BookId);
        //        vm.Review = review;
        //        return View("detail", vm);
        //    }

        //    review.AppUserId = user.Id;
        //    review.CreatedAt = DateTime.Now;

        //    _context.BookReviews.Add(review);
        //    _context.SaveChanges();

        //    return RedirectToAction("detail", new { id = review.BookId });
        //}
    }
}
