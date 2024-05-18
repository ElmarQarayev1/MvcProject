﻿using System;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcProject.Data;
using MvcProject.Models;
using MvcProject.ViewModels;

namespace MvcProject.Controllers
{
	public class AccountController:Controller
	{
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(AppDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Register()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Register(MemberRegisterViewModel member)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (_userManager.Users.Any(x => x.NormalizedEmail == member.Email.ToUpper()))
            {
                ModelState.AddModelError("Email", "Email is already taken");
                return View();
            }
            AppUser appUser = new AppUser()
            {
                UserName = member.UserName,
                Email = member.Email,
                FullName = member.FullName
            };
            var result = await _userManager.CreateAsync(appUser, member.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    if (item.Code == "DuplicateUserName")
                        ModelState.AddModelError("UserName", "UserName is already taken");
                    else
                        ModelState.AddModelError("", item.Description);
                }
                return View();
            }
            await _userManager.AddToRoleAsync(appUser, "member");
            return RedirectToAction("login", "account");
        }
        public IActionResult Login()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(MemberLoginViewModel member, string returnUrl)
        {
            if (member.Password == null)
            {
                ModelState.AddModelError("Password", "Password mustn't be null");
                return View(member);
            }
            AppUser appUser = await _userManager.FindByEmailAsync(member.Email);

            if (appUser == null || !await _userManager.IsInRoleAsync(appUser, "member"))
            {
                ModelState.AddModelError("", "Email or Password is incorrect");
                return View(member);
            }
            var result = await _signInManager.PasswordSignInAsync(appUser, member.Password, false, true);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Email or Password is incorrect");
                return View(member);
            }
            return returnUrl != null ? Redirect(returnUrl) : RedirectToAction("index", "home");
        }
        [Authorize(Roles = "member")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

    }
}
