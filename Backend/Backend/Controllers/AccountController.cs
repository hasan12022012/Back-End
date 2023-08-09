using Backend.DataAccessLayer;
using Backend.Helpers.Enums;
using Backend.Models;
using Backend.Services.Interfaces;
using Backend.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _appDbContext;
        private readonly IEmailService _emailService;
        private readonly IFileService _fileService;

        public AccountController
            (UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext appDbContext,
            IEmailService emailService,
            IFileService fileService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _appDbContext = appDbContext;
            _emailService = emailService;
            _fileService = fileService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid) return View(registerVM);

            string otp = GenerateOTP();

            AppUser user = new AppUser
            {
                Name = registerVM.Name,
                Surname = registerVM.Surname,
                UserName = registerVM.Username,
                Email = registerVM.Email,
                OTP = otp
            };

            IdentityResult result = await _userManager.CreateAsync(user, registerVM.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(registerVM);
            }

            await _userManager.AddToRoleAsync(user, Roles.Member.ToString());

            string body = string.Empty;
            string path = "wwwroot/assets/templates/verify.html";
            string subject = "Verify Email";

            body = _fileService.ReadFile(path, body);

            body = body.Replace("{{otp}}", otp);
            body = body.Replace("{{name}}", user.Name);
            body = body.Replace("{{surname}}", user.Surname);

            _emailService.Send(user.Email, subject, body);

            return RedirectToAction(nameof(VerifyEmail), new { Email = user.Email });
        }

        public IActionResult VerifyEmail(string email)
        {
            ConfirmAccountVM confirmAccount = new ConfirmAccountVM()
            {
                Email = email
            };

            return View(confirmAccount);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmEmail(ConfirmAccountVM confirmAccount)
        {
            AppUser existUser = await _userManager.FindByEmailAsync(confirmAccount.Email);

            if (existUser == null) return NotFound();

            if (existUser.OTP != confirmAccount.OTP || string.IsNullOrEmpty(confirmAccount.OTP))
            {
                TempData["Error"] = "Wrong OTP";
                return RedirectToAction(nameof(VerifyEmail), new { Email = confirmAccount.Email });
            }

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(existUser);

            await _userManager.ConfirmEmailAsync(existUser, token);

            await _signInManager.SignInAsync(existUser, false);

            return RedirectToAction(nameof(Login));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendOTP(string email)
        {
            string otp = GenerateOTP();

            AppUser existUser = await _userManager.FindByEmailAsync(email);
            existUser.OTP = otp;

            await _userManager.UpdateAsync(existUser);

            string body = string.Empty;
            string path = "wwwroot/assets/templates/verify.html";
            string subject = "Verify Email";

            body = _fileService.ReadFile(path, body);

            body = body.Replace("{{otp}}", otp);
            body = body.Replace("{{name}}", existUser.Name);
            body = body.Replace("{{surname}}", existUser.Surname);

            _emailService.Send(existUser.Email, subject, body);

            return RedirectToAction(nameof(VerifyEmail), new { Email = existUser.Email });
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM changePassword)
        {
            if (!ModelState.IsValid) return View();

            AppUser existUser = await _userManager.FindByNameAsync(User.Identity.Name);

            IdentityResult result = await _userManager.ChangePasswordAsync(existUser, changePassword.CurrentPassword, changePassword.NewPassword);

            if (result.Succeeded)
            {
                ViewBag.IsSuccess = true;
                return View(changePassword);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(changePassword);
            }
        }

        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM forgetPassword)
        {
            if (!ModelState.IsValid) return View(forgetPassword);

            AppUser existUser = await _userManager.FindByEmailAsync(forgetPassword.Email);

            if (existUser is null)
            {
                ModelState.AddModelError("Email", "User not found!");
                return View(forgetPassword);
            }

            string token = await _userManager.GeneratePasswordResetTokenAsync(existUser);

            string link = Url.Action(nameof(ResetPassword), "Account", new { userId = existUser.Id, token }, Request.Scheme, Request.Host.ToString());

            string body = string.Empty;
            string path = "wwwroot/assets/templates/ForgetPasswordVerify.html";
            string subject = "Verify Email";

            body = _fileService.ReadFile(path, body);

            body = body.Replace("{{link}}", link);
            body = body.Replace("{{name}}", existUser.Name);
            body = body.Replace("{{surname}}", existUser.Surname);

            _emailService.Send(existUser.Email, subject, body);


            return RedirectToAction(nameof(ResetPasswordVerifyEmail));
        }

        public IActionResult ResetPasswordVerifyEmail()
        {
            return View();
        }

        public IActionResult ResetPassword(string userId, string token)
        {
            return View(new ResetPasswordVM { Token = token, UserId = userId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPassword)
        {
            if (!ModelState.IsValid) return View(resetPassword);

            AppUser existUser = await _userManager.FindByIdAsync(resetPassword.UserId);

            if (existUser == null) return NotFound();

            if (await _userManager.CheckPasswordAsync(existUser, resetPassword.Password))
            {
                ModelState.AddModelError("", "This password already exist!");
                return View(resetPassword);
            }

            await _userManager.ResetPasswordAsync(existUser, resetPassword.Token, resetPassword.Password);

            return RedirectToAction(nameof(Login));
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }

            AppUser user = await _userManager.FindByEmailAsync(loginVM.UsernameOrEmail);

            if (user is null)
            {
                user = await _userManager.FindByNameAsync(loginVM.UsernameOrEmail);
            }

            if (user is null)
            {
                ModelState.AddModelError("", "Email or username is wrong!");

                return View(loginVM);
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Email or username is wrong!");

                return View(loginVM);
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        //[Authorize(Roles = "SuperAdmin")]
        public async Task CreateRoles()
        {
            foreach (var role in Enum.GetValues(typeof(Roles)))
            {
                if (!await _roleManager.RoleExistsAsync(role.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole { Name = role.ToString() });
                }
            }
        }

        private static string GenerateOTP()
        {
            Random random = new Random();
            int otpNumber = random.Next(100000, 999999);
            return otpNumber.ToString();
        }
    }
}
