using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SWZSR.Data;
using SWZSR.Models;
using SWZSR.ViewModels;

namespace SWZSR.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManageAccountController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private IUserValidator<ApplicationUser> _userValidator;
        private IPasswordHasher<ApplicationUser> _passwordHasher;
        private IPasswordValidator<ApplicationUser> _passwordValidator;
        private readonly ApplicationDbContext _db;

        public ManageAccountController(UserManager<ApplicationUser> userMngr, IUserValidator<ApplicationUser> userVldtr, IPasswordHasher<ApplicationUser> passwordHshr, IPasswordValidator<ApplicationUser> passwordVldtr, ApplicationDbContext context)
        {
            _userManager = userMngr;
            _userValidator = userVldtr;
            _passwordHasher = passwordHshr;
            _passwordValidator = passwordVldtr;
            _db = context;
        }

        public async Task<ViewResult> Index()
        {
            var clients = await _userManager.GetUsersInRoleAsync("Client");
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            var mechanics = await _userManager.GetUsersInRoleAsync("Mechanic");

            var model = new ManageAccountsViewModel
            {
                Admins = admins,
                Mechanics = mechanics,
                Clients = clients
            };

            return View(model);
        }

        public IActionResult ResetPassword()
        {
            return View();
        }

        public ViewResult AddAccount()
        {
            var model = new AddAccountViewModel
            {
                Roles = _db.Roles.ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddAccount(AddAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.Email,
                    Firstname = model.FirstName,
                    Lastname = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    EmailConfirmed = true
                };

                IdentityResult result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    if(model.RoleName != null)
                    {
                        IdentityResult roleResult = await _userManager.AddToRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                        {
                            AddErrorsFromResult(roleResult);
                        }
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            model.Roles = _db.Roles.ToList();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "Nie znaleziono użytkownika.");
            }
            return View("Index", _userManager.Users);
        }

        public IActionResult BlockAccount()
        {
            return RedirectToAction("Index");
        }

        // GET: ChangeAccount
        public async Task<IActionResult> ChangeAccount(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            var model = new ManageAccountViewModel
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.Firstname,
                LastName = user.Lastname,
                Roles = _db.Roles.ToList(),
                UserRoles = await _userManager.GetRolesAsync(user)
            };
            if (user != null)
            {
                return View(model);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeAccount(ManageAccountViewModel model)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(model.Id);
            if (user != null)
            {
                user.Email = model.Email;
                IdentityResult validEmail = await _userValidator.ValidateAsync(_userManager, user);
                if (!validEmail.Succeeded)
                {
                    AddErrorsFromResult(validEmail);
                }
                IdentityResult validPass = null;
                if (!string.IsNullOrEmpty(model.Password))
                {
                    validPass = await _passwordValidator.ValidateAsync(_userManager, user, model.Password);
                    if (validPass.Succeeded)
                    {
                        user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);
                    }
                    else
                    {
                        AddErrorsFromResult(validPass);
                    }
                }

                if (model.RoleName != null)
                {
                    IdentityResult roleResult = await _userManager.AddToRoleAsync(user, model.RoleName);
                    if (!roleResult.Succeeded)
                    {
                        AddErrorsFromResult(roleResult);
                    }
                }

                user.Firstname = model.FirstName;
                user.Lastname = model.LastName;
                IdentityResult validInfo = await _userValidator.ValidateAsync(_userManager, user);
                if (!validInfo.Succeeded)
                {
                    AddErrorsFromResult(validInfo);
                }

                if ((validEmail.Succeeded && validInfo.Succeeded && validPass == null) || (validEmail.Succeeded && validInfo.Succeeded && model.Password != string.Empty && validPass.Succeeded))
                {
                    IdentityResult result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        AddErrorsFromResult(result);
                    }
                }

            }
            else
            {
                ModelState.AddModelError("", "Nie znaleziono użytkownika.");
            }
            model.UserRoles = await _userManager.GetRolesAsync(user);
            return View(model);
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach(IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
    }
}