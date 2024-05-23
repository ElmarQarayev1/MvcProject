using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcProject.Areas.Manage.ViewModels;
using MvcProject.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MvcProject.Areas.Manage.Controllers
{
    [Area("manage")]
    public class AccountController:Controller
	{
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> CreateRoles()
        {
            await _roleManager.CreateAsync(new IdentityRole("admin"));
            await _roleManager.CreateAsync(new IdentityRole("member"));
            await _roleManager.CreateAsync(new IdentityRole("superadmin"));
            return Ok();
        }
        public async Task<IActionResult> CreateAdmin()
        {
            AppUser appUser = new AppUser()
            {
                UserName = "admin",
            };
            var result = await _userManager.CreateAsync(appUser, "Admin123");
            await _userManager.AddToRoleAsync(appUser, "admin");
            return Json(result);
        }
        public async Task<IActionResult> CreateSuperAdmin()
        {
            AppUser appUser = new AppUser()
            {
                UserName = "superadmin",
                Email="superadmin@gmail.com"
              
            };
            var result = await _userManager.CreateAsync(appUser, "SuperAdmin123");
            await _userManager.AddToRoleAsync(appUser, "superadmin");
            return Json(result);
        }

        public IActionResult Login()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(AdminLoginViewModel admin, string returnUrl)
        {
            if (admin == null)
            {
                ModelState.AddModelError("", "UserName or Password is not true");
                return View();
            }

            AppUser appUser = await _userManager.FindByNameAsync(admin.UserName);
            if (appUser == null)
            {
                ModelState.AddModelError("", "UserName or Password incorrect");
                return View();
            }
            if (admin == null || (!await _userManager.IsInRoleAsync(appUser, "admin") && !await _userManager.IsInRoleAsync(appUser, "superadmin")))
            {
                ModelState.AddModelError("", "UserName or Password incorrect");
                return View();
            }
            var result = await _signInManager.PasswordSignInAsync(appUser, admin.Password, admin.RememberMe, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "UserName or Password is not true");
                return View();
            }

            return returnUrl != null ? Redirect(returnUrl) : RedirectToAction("index", "dashboard");
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("login", "account");
        }
        public IActionResult AdminCreateByS()
        {
            return View();
        }
        [Authorize(Roles ="superadmin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> AdminCreateByS(AdminCreateViewModel av)
        {
            AppUser appUser = new AppUser()
            {
                UserName = av.UserName,
                Email="admin@gmail.com"
                
               
            };
            var result = await _userManager.CreateAsync(appUser, av.Password);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors )
                {
                    ModelState.AddModelError("", err.Description);
                    return View();
                }
            }
            await _userManager.AddToRoleAsync(appUser, "admin");
            return RedirectToAction("index","dashboard");
         }
    }
}

