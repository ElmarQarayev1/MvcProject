using System;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcProject.Data;
using MvcProject.Models;
using MvcProject.ViewModels;
using MvcProject.Services;
using System.Security.Claims;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using MvcProject.Models.Enum;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

namespace MvcProject.Controllers
{
	public class AccountController:Controller
	{
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
       private readonly EmailService _emailService;

        public AccountController(AppDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, EmailService emailService)
        {
            _emailService = emailService;
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
            if (_userManager.Users.Any(x => x.Email == member.Email.ToLower()))
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
                        ModelState.AddModelError("UserName", "UserName is already registired");
                    else
                        ModelState.AddModelError("", item.Description);
                }
                return View();
            }
            await _userManager.AddToRoleAsync(appUser, "member");

            var token =   _userManager.GenerateEmailConfirmationTokenAsync(appUser).Result;

            var url = Url.Action("VerifyEmail", "Account", new { userId=appUser.Id, token = token }, Request.Scheme);

           // TempData["Confirm"] = member.Email;

            var subject = "Confirm your email address";

            var body = $"<h1><a href=\"{url}\"> confirm your email</a></h1>";

            _emailService.Send(appUser.Email, subject, body);

            return RedirectToAction("index", "home");
        }

        public async Task<IActionResult> VerifyEmail(string userId, string token)
        {
            var appUser = await _userManager.FindByIdAsync(userId);

            if (userId==null || token == null)
            {
                return RedirectToAction("notfound", "error");
            }

            if (appUser == null)
            {
                return RedirectToAction("notfound", "Error");
            }

            var r = await _userManager.ConfirmEmailAsync(appUser, token);

            if (!r.Succeeded)
            {
                return RedirectToAction("notfound", "Error");
            }
            return RedirectToAction("login");
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

            if (!await _userManager.IsEmailConfirmedAsync(appUser))
            {
                ModelState.AddModelError("", "Confirm your email address");
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



        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }



        public IActionResult ForgetPassword()
        {
            return View();
        }

     
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult ForgetPassword(ForgetPasswordViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = _userManager.FindByEmailAsync(vm.Email).Result;

            if (user == null || !_userManager.IsInRoleAsync(user, "member").Result)
            {
                ModelState.AddModelError("", "Account does not exist");
                return View(vm);
            }
            var token = _userManager.GeneratePasswordResetTokenAsync(user).Result;
            var url = Url.Action("Verify", "Account", new { email = vm.Email, token = token }, Request.Scheme);
            TempData["EmailSent"] = vm.Email;

            var subject = "Reset Password Link";
              var body = $"<h1><a href=\"{url}\"> reset your password</a></h1>";
               _emailService.Send(user.Email, subject, body);
            return View();
        }


        public IActionResult Verify(string email, string token)
        {
            var user = _userManager.FindByEmailAsync(email.ToLower()).Result;

            if (user == null || !_userManager.IsInRoleAsync(user, "member").Result)
            {
                return RedirectToAction("NotFound", "Error");
            }

            if (!_userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token).Result)
            {
                return RedirectToAction("NotFound", "Error");
            }
            TempData["email"] = email;
            TempData["token"] = token;

            return RedirectToAction("ResetPassword");
        }




        public async Task<IActionResult> Profile()
        {
            AppUser? user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("login", "account");
            }

            var applications = await _context.Applications
                                             .Include(x => x.Course)
                                             .Include(x => x.AppUser)
                                             .Where(x => x.AppUserId == user.Id && x.Status != ApplicationStatus.Canceled)
                                             .ToListAsync();

            bool hasPassword = await _userManager.HasPasswordAsync(user);
            bool isGoogleUser = (await _userManager.GetLoginsAsync(user)).Any(login => login.LoginProvider == "Google");

            ProfileEditViewModel profileVM = new ProfileEditViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                UserName = user.UserName,
                Applications = applications,
                HasPassword = hasPassword,
                IsGoogleUser = isGoogleUser
            };

            return View(profileVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileEditViewModel profileEditViewModel)
        {
            AppUser? appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
            {
                TempData["ProfileUpdateError"] = "User not found. Please log in again.";
                return RedirectToAction("login", "account");
            }

            bool hasPassword = await _userManager.HasPasswordAsync(appUser);
            bool isGoogleUser = (await _userManager.GetLoginsAsync(appUser)).Any(login => login.LoginProvider == "Google");

            profileEditViewModel.HasPassword = hasPassword;
            profileEditViewModel.IsGoogleUser = isGoogleUser;

            if (!ModelState.IsValid)
            {
                TempData["ProfileUpdateError"] = "Please correct the errors and try again.";
                return View(profileEditViewModel);
            }

            appUser.UserName = profileEditViewModel.UserName;
            appUser.Email = profileEditViewModel.Email;
            appUser.FullName = profileEditViewModel.FullName;

            if (_userManager.Users.Any(x => x.Id != appUser.Id && x.NormalizedEmail == profileEditViewModel.Email.ToUpper()))
            {
                ModelState.AddModelError("Email", "Email is already taken");
                TempData["ProfileUpdateError"] = "Email is already taken.";
                return View(profileEditViewModel);
            }

            if (!string.IsNullOrEmpty(profileEditViewModel.NewPassword))
            {
                if (isGoogleUser || !hasPassword)
                {
                    var addPasswordResult = await _userManager.AddPasswordAsync(appUser, profileEditViewModel.NewPassword);

                    if (!addPasswordResult.Succeeded)
                    {
                        foreach (var error in addPasswordResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        TempData["ProfileUpdateError"] = "Failed to add new password.";
                        return View(profileEditViewModel);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(profileEditViewModel.CurrentPassword))
                    {
                        ModelState.AddModelError("CurrentPassword", "Current password is required.");
                        TempData["ProfileUpdateError"] = "Please correct the errors and try again.";
                        return View(profileEditViewModel);
                    }

                    var changePasswordResult = await _userManager.ChangePasswordAsync(appUser, profileEditViewModel.CurrentPassword, profileEditViewModel.NewPassword);

                    if (!changePasswordResult.Succeeded)
                    {
                        foreach (var error in changePasswordResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        TempData["ProfileUpdateError"] = "Failed to change password.";
                        return View(profileEditViewModel);
                    }
                }
            }

            var updateResult = await _userManager.UpdateAsync(appUser);

            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                TempData["ProfileUpdateError"] = "Failed to update profile.";
                return View(profileEditViewModel);
            }

            await _signInManager.SignInAsync(appUser, isPersistent: false);
            TempData["ProfileUpdateSuccess"] = "Profile updated successfully!";
            return RedirectToAction("profile");
        }



        public IActionResult ResetPassword()
        {
            return View();
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel vm)
        {
            TempData["email"] = vm.Email;

            TempData["token"] = vm.Token;

            if (!ModelState.IsValid) return View(vm);

            AppUser? user = _userManager.FindByEmailAsync(vm.Email).Result;


            if (user == null || !_userManager.IsInRoleAsync(user, "member").Result)
            {
                ModelState.AddModelError("", "Account is not exist");

                return View();
            }


            if (!_userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", vm.Token).Result)
            {
                ModelState.AddModelError("", "Account is not exist");
                return View();
            }


            var result = _userManager.ResetPasswordAsync(user, vm.Token, vm.NewPassword).Result;


            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View();
            }


            return RedirectToAction("login");
        }

     
        public IActionResult GoogleLogin(string returnUrl = null)
        {

            returnUrl = returnUrl ?? Url.Content("~/");


            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse", "Account", new { returnUrl }) };


            return Challenge(properties, "Google");
        }


        public async Task<IActionResult> GoogleResponse(string returnUrl = null)
        {
            var result = await HttpContext.AuthenticateAsync("Google");

            if (result?.Principal == null)
            {
                Console.WriteLine("Authentication failed. Principal is null.");

                return RedirectToAction("Login", "Account", new { message = "Authentication failed. Please try again." });
            }

            var emailClaim = result.Principal.FindFirst(claim => claim.Type == ClaimTypes.Email);


            var email = emailClaim?.Value;

            if (email == null)
            {
                Console.WriteLine("Email claim not found.");


                return RedirectToAction("Login", "Account", new { message = "Email claim not found. Please try again." });
            }

            var appUser = await _userManager.FindByEmailAsync(email);

            if (appUser == null)
            {
                var username = GenerateUserNameFromEmail(email);

                appUser = new AppUser
                {
                    UserName = username,
                    Email = email,
                    FullName = username,
                    EmailConfirmed = true,
                };


                var createResult = await _userManager.CreateAsync(appUser);


                if (!createResult.Succeeded)
                {
                    foreach (var error in createResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View("Login", new { message = "User creation failed. Please try again." });
                }
               

                await _userManager.AddToRoleAsync(appUser, "member");
            }
            else if (!appUser.EmailConfirmed)
            {

                appUser.EmailConfirmed = true;
                await _userManager.UpdateAsync(appUser);
            }

            await _signInManager.SignInAsync(appUser, isPersistent: false);

            return returnUrl != null ? Redirect(returnUrl) : RedirectToAction("Index", "Home");
        }

        private string GenerateUserNameFromEmail(string email)
        {
            var atIndex = email.IndexOf('@');
            return atIndex > 0 ? email.Substring(0, atIndex) : email;
        }

    }
}

