using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SWZSR.Infrastructure;
using SWZSR.Infrastructure.Alerts;
using SWZSR.Models;
using SWZSR.ViewModels;

namespace SWZSR.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private static IHostingEnvironment _env;
        private AlertService _alertService { get; }
        private IUserValidator<ApplicationUser> _userValidator;
        private IPasswordHasher<ApplicationUser> _passwordHasher;
        private IPasswordValidator<ApplicationUser> _passwordValidator;
        private NotificationManager _notification;

        public AccountController(UserManager<ApplicationUser> userMngr, SignInManager<ApplicationUser> signInMngr, IHostingEnvironment hostingEnvironment, 
            AlertService alertService, IUserValidator<ApplicationUser> userVldtr, IPasswordHasher<ApplicationUser> passwordHshr, 
            IPasswordValidator<ApplicationUser> passwordVldtr, NotificationManager notification)
        {
            _userManager = userMngr;
            _signInManager = signInMngr;
            _env = hostingEnvironment;
            _alertService = alertService;
            _userValidator = userVldtr;
            _passwordHasher = passwordHshr;
            _passwordValidator = passwordVldtr;
            _notification = notification;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRegisterViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(model.LoginViewModel.Email);
                if (user != null)
                {
                    await _signInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user, model.LoginViewModel.Password, model.LoginViewModel.RememberMe, false);

                    if (result.Succeeded)
                    {
                        return Redirect(returnUrl ?? "/");
                    }
                    else if (user.EmailConfirmed == false)
                    {
                        _alertService.Danger("Konto nieaktywne. Aktywuj konto klikając na link w wiadomości email wysłanej podczas rejestracji.");
                    }
                }
                else
                    _alertService.Danger("Niepoprawny email lub hasło.");
            }
            else
                _alertService.Danger("Nie wypełniono wszystkich pól formularza lub podane dane są nieprawidłowe.");
            
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(LoginRegisterViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser {
                    UserName = model.RegisterViewModel.Email,
                    Email = model.RegisterViewModel.Email,
                    Firstname = model.RegisterViewModel.FirstName,
                    Lastname = model.RegisterViewModel.LastName,
                    PhoneNumber = model.RegisterViewModel.PhoneNumber
                };
                var result = await _userManager.CreateAsync(user, model.RegisterViewModel.Password);
                if (result.Succeeded)
                {
                    IdentityResult roleResult = await _userManager.AddToRoleAsync(user, "Client");
                    if (!roleResult.Succeeded)
                    {
                        ModelState.AddModelError("", "Niepowodzenie dodawania roli.");
                    }

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, emailToken = code }, Request.Scheme);

                    //await _emailSender.SendEmailAsync(model.RegisterViewModel.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    //NotificationManager _emailService = new NotificationManager();
                    await _notification.SendEmail(MailType.Registration, model.RegisterViewModel.Email, _env, $"kliknij <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>w ten link</a>.");

                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    _alertService.Success("Na Twój email została wysłana wiadomość z linkiem potwierdzającym rejestrację. Kliknij w niego, aby aktywować konto.");
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            _alertService.Danger("Nie wypełniono wszystkich pól formularza lub podane dane są nieprawidłowe.");
            return View("Login", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/ConfirmEmail?
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string emailToken)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if(user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, emailToken);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _alertService.Success("Twoje konto zostało aktywowane, a Ty zalogowany!");
                    return RedirectToAction("MyAccount");
                }
                else
                {
                    _alertService.Danger("Nieprawidłowy token.");
                    return RedirectToAction("Login");
                }
            }


            _alertService.Warning("Nie znaleziono użytkownika.");
            return RedirectToAction("Login");
        }

        // GET: /Account/MyAccount
        public async Task<IActionResult> MyAccount()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            string userRoles = String.Join(", ", await _userManager.GetRolesAsync(user));

            var model = new ManageCredentialsViewModel
            {
                UserRole = userRoles,
                Email = user.Email,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                PhoneNumber = user.PhoneNumber
            };

            return View(model);
        }

        // GET: /Account/ChangeProfile
        public async Task<IActionResult> ChangeProfile()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var model = new ChangeCredentialsViewModel
            {
                FirstName = user.Firstname,
                LastName = user.Lastname,
                PhoneNumber = user.PhoneNumber
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeProfile(ChangeCredentialsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                user.Firstname = model.FirstName;
                user.Lastname = model.LastName;
                user.PhoneNumber = model.PhoneNumber;
                IdentityResult validInfo = await _userValidator.ValidateAsync(_userManager, user);
                if (validInfo.Succeeded)
                {
                    IdentityResult result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        _alertService.Success("Zmieniono dane użytkownika");
                        return RedirectToAction("MyAccount");
                    }
                    else
                    {
                        _alertService.Danger(result.Errors.ToString());
                        return View(model);
                    }
                }
                else
                {
                    _alertService.Danger(validInfo.Errors.ToString());
                    return View(model);
                }
            }
            else
            {
                _alertService.Danger("Nie wypełniono wszystkich pól formularza lub podane dane są nieprawidłowe.");
                return View(model);
            }
        }

        // GET: /Account/ChangePassword
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                IdentityResult validPass = null;
                if (!string.IsNullOrEmpty(model.Password))
                {
                    validPass = await _userManager.ChangePasswordAsync(user, model.Password, model.NewPassword);
                    if (validPass.Succeeded)
                    {
                        user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);
                        _alertService.Success("Pomyślnie zmieniono hasło!");
                        return RedirectToAction("MyAccount");
                    }
                    else
                    {
                        _alertService.Danger(validPass.Errors.ToString());
                        return View(model);
                    }
                }
            }
            _alertService.Danger("Nie wypełniono wszystkich pól formularza lub podane hasła są nieprawidłowe.");
            return View(model);
        }
    }
}