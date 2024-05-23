using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcProject.Areas.Manage.ViewModels;
using MvcProject.Data;
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
        private readonly AppDbContext _context;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager,AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
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
            if (!ModelState.IsValid)
            {
                return View(av);
            }
            AppUser appUser = new AppUser()
            {
                UserName = av.UserName,
                                  
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
        [Authorize(Roles = "superadmin")]
        public async Task<IActionResult> ShowAdmin()
        {
            var adminRole = await _roleManager.FindByNameAsync("admin");
            if (adminRole == null)
            {
                return NotFound("Admin role not found");
            }
            var admins = await _userManager.GetUsersInRoleAsync("admin");
            var adminViewModels = admins.Select(admin => new AdminViewModel
            {
                UserName = admin.UserName,
                Email = admin.Email
            }).ToList();
            return View(adminViewModels);
        }
        [Authorize(Roles = "admin, superadmin")]
        public async Task<IActionResult> Profile()
        {
            AppUser? user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            AdminProfileEditViewModel profileVM = new AdminProfileEditViewModel
            {
               
                UserName = user.UserName
            };

            return View(profileVM);
        }

        [Authorize(Roles = "admin, superadmin")]
        [HttpPost]
        public async Task<IActionResult> Profile(AdminProfileEditViewModel profileVM)
        {
            if (!ModelState.IsValid)
            {
                return View(profileVM);
            }

            AppUser? appUser = await _userManager.GetUserAsync(User);

            if (appUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            appUser.UserName = profileVM.UserName;
           
            if (!string.IsNullOrEmpty(profileVM.CurrentPassword) && !string.IsNullOrEmpty(profileVM.NewPassword))
            {
                var passwordResult = await _userManager.ChangePasswordAsync(appUser, profileVM.CurrentPassword, profileVM.NewPassword);

                if (!passwordResult.Succeeded)
                {
                    foreach (var error in passwordResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(profileVM);
                }
            }

            var result = await _userManager.UpdateAsync(appUser);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    if (err.Code == "DuplicateUserName")
                    {
                        ModelState.AddModelError("UserName", "UserName is already taken");
                    }
                    else
                    {
                        ModelState.AddModelError("", err.Description);
                    }
                }
                return View(profileVM);
            }
         
            await _signInManager.SignInAsync(appUser, false);
    
            return RedirectToAction("login","account");
        }

    }
}

