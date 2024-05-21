using System;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcProject.Data;
using MvcProject.Models;
using MvcProject.ViewModels;
using MvcProject.Services;

namespace MvcProject.Controllers
{
	public class AccountController:Controller
	{
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
      //  private readonly EmailService _emailService;

        public AccountController(AppDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager /*EmailService emailService*/)
        {
            //_emailService = emailService;
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
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ForgetPassword(ForgetPasswordViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = _userManager.FindByEmailAsync(vm.Email.ToLower()).Result;

            if (user == null || !_userManager.IsInRoleAsync(user, "member").Result)
            {
                ModelState.AddModelError("", "Account does not exist");
                return View(vm);
            }
            var token = _userManager.GeneratePasswordResetTokenAsync(user).Result;

            var url = Url.Action("Verify", "Account", new { email = vm.Email, token = token }, Request.Scheme);
            TempData["EmailSent"] = vm.Email;

            // send email
            return Json(new { url = url });
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

        public IActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel vm)
        {
            var user = _userManager.FindByEmailAsync(vm.Email.ToLower()).Result;

            if (user == null || !_userManager.IsInRoleAsync(user, "member").Result)
            {
                ModelState.AddModelError("", "Account does not exist");
                return View(vm);
            }

            if (!_userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", vm.Token).Result)
            {
                ModelState.AddModelError("", "Invalid token");
                return View(vm);
            }

            var result = _userManager.ResetPasswordAsync(user, vm.Token, vm.NewPassword).Result;

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View(vm);
            }

            return RedirectToAction("Login");
        }


        //[HttpPost]
        //public IActionResult ForgetPassword(ForgetPasswordViewModel viewmodel)
        //{
        //    if (!ModelState.IsValid) return View(viewmodel);

        //    AppUser? user = _userManager.FindByEmailAsync(viewmodel.Email).Result;

        //    if (user == null || !_userManager.IsInRoleAsync(user, "member").Result)
        //    {
        //        ModelState.AddModelError("", "Account isn't found");
        //        return View();
        //    }

        //    var token = _userManager.GeneratePasswordResetTokenAsync(user).Result;


        //    var url = Url.Action("verify", "account", new { email = viewmodel.Email, token = token }, Request.Scheme);
        //    TempData["EmailSent"] = viewmodel.Email;


        //    //var emailMessage = new MimeMessage();
        //    //emailMessage.From.Add(MailboxAddress.Parse("elmar@gmail.com"));
        //    //emailMessage.To.Add(MailboxAddress.Parse(viewmodel.Email));
        //    //emailMessage.Subject = "Reset Password";
        //    //emailMessage.Body = new TextPart("html")
        //    //{
        //    //    Text = $"<a href='{url}'>Reset Password</a>"
        //    //};
        //    //using (var client = new MailKit.Net.Smtp.SmtpClient())
        //    //{
        //    //    client.Connect("elmar@gmail.com", 587, true); 
        //    //    client.Authenticate("celestino.jakubowski@ethereal.email", "qdD9uFrHFWdaTYBSQb");
        //    //    client.Send(emailMessage);
        //    //    client.Disconnect(true);
        //    //}

        //    //SocketException: nodename nor servname provided, or not known

        //    return Json(new { url = url });
        //}


        //[HttpPost]
        //public IActionResult ForgetPassword(ForgetPasswordViewModel vm)
        //{
        //    if (!ModelState.IsValid) return View(vm);

        //    AppUser? user = _userManager.FindByEmailAsync(vm.Email).Result;

        //    if (user == null || !_userManager.IsInRoleAsync(user, "member").Result)
        //    {
        //        ModelState.AddModelError("", "Account is not exist");
        //        return View();
        //    }

        //    var token = _userManager.GeneratePasswordResetTokenAsync(user).Result;


        //    var url = Url.Action("verify", "account", new { email = vm.Email, token = token }, Request.Scheme);
        //    TempData["EmailSent"] = vm.Email;

        //    //send email

        //    return Json(new { url = url });
        //}


        //[HttpPost]
        //public IActionResult ForgetPassword(ForgetPasswordViewModel vm)
        //{
        //    if (!ModelState.IsValid) return View(vm);

        //    AppUser? user = _userManager.FindByEmailAsync(vm.Email).Result;

        //    if (user == null || !_userManager.IsInRoleAsync(user, "member").Result)
        //    {
        //        ModelState.AddModelError("", "Account is not exist");
        //        return View();
        //    }
        //    var token = _userManager.GeneratePasswordResetTokenAsync(user).Result;

        //    var url = Url.Action("verify", "account", new { email = vm.Email, token = token }, Request.Scheme);
        //    TempData["EmailSent"] = vm.Email;

        //    var subject = "Reset Password Link";
        //    var body = $"<h1>Click <a href=\"{url}\">here</a> to reset your password</h1>";
        //    _emailService.Send(user.Email, subject, body);
        //    return View();
        //}
        //public IActionResult Verify(string email, string token)
        //{
        //    AppUser? user = _userManager.FindByEmailAsync(email).Result;

        //    if (user == null || !_userManager.IsInRoleAsync(user, "member").Result)
        //    {
        //        return RedirectToAction("notfound", "error");
        //    }

        //    if (!_userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token).Result)
        //    {
        //        return RedirectToAction("notfound", "error");
        //    }

        //    TempData["email"] = email;

        //    TempData["token"] = token;

        //    return RedirectToAction("resetPassword");
        //}

        //public IActionResult Verify(string email, string token)
        //{
        //    AppUser? user = _userManager.FindByEmailAsync(email).Result;

        //    if (user == null || !_userManager.IsInRoleAsync(user, "member").Result)
        //    {
        //        return RedirectToAction("notfound", "error");
        //    }
        //    if (!_userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token).Result)
        //    {
        //        return RedirectToAction("notfound", "error");
        //    }
        //    TempData["email"] = email;
        //    TempData["token"] = token;

        //    return RedirectToAction("resetPassword");
        //}

        //public IActionResult ResetPassword()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public IActionResult ResetPassword(ResetPasswordViewModel vm)
        //{
        //    TempData["email"] = vm.Email;
        //    TempData["token"] = vm.Token;
        //    if (!ModelState.IsValid) return View(vm);

        //    AppUser? user = _userManager.FindByEmailAsync(vm.Email).Result;

        //    if (user == null || !_userManager.IsInRoleAsync(user, "member").Result)
        //    {
        //        ModelState.AddModelError("", "Account is not exist");
        //        return View();
        //    }

        //    if (!_userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", vm.Token).Result)
        //    {
        //        ModelState.AddModelError("", "Account is not exist");
        //        return View();
        //    }
        //    var result = _userManager.ResetPasswordAsync(user, vm.Token, vm.NewPassword).Result;

        //    if (!result.Succeeded)
        //    {
        //        foreach (var item in result.Errors)
        //        {
        //            ModelState.AddModelError("", item.Description);
        //        }
        //        return View();
        //    }

        //    return RedirectToAction("login");
        //}
        //[HttpPost]
        //public IActionResult ResetPassword(ResetPasswordViewModel vm)
        //{
        //    AppUser? user = _userManager.FindByEmailAsync(vm.Email).Result;

        //    if (user == null || !_userManager.IsInRoleAsync(user, "member").Result)
        //    {
        //        ModelState.AddModelError("", "Account is not exist");
        //        return View();
        //    }

        //    if (!_userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", vm.Token).Result)
        //    {
        //        ModelState.AddModelError("", "Account is not exist");
        //        return View();
        //    }

        //    var result = _userManager.ResetPasswordAsync(user, vm.Token, vm.NewPassword).Result;

        //    if (!result.Succeeded)
        //    {
        //        foreach (var item in result.Errors)
        //        {
        //            ModelState.AddModelError("", item.Description);
        //        }
        //        return View();
        //    }



        //    return RedirectToAction("login");
        //}

        //public IActionResult Users()
        //{
        //    var users = _userManager.Users.ToList();

        //    return View(users);
        //}

    }
}

