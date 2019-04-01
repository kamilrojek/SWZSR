using Microsoft.AspNetCore.Identity;
using SWZSR.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SWZSR.ViewModels
{
    public class ManageAccountsViewModel
    {
        public IList<ApplicationUser> Admins { get; set; }
        public IList<ApplicationUser> Mechanics { get; set; }
        public IList<ApplicationUser> Clients { get; set; }
    }

    public class ManageAccountViewModel
    {
        [EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string Id { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public List<IdentityRole> Roles { get; set; }
        public IList<string> UserRoles { get; set; }
        public string RoleName { get; set; }
    }

    public class AddAccountViewModel
    {
        [Required]
        [Display(Name = "Imię")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Adres email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Numer telefonu")]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Hasło")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Powtórz hasło")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Hasła różnią się")]
        public string ConfirmPassword { get; set; }

        public List<IdentityRole> Roles { get; set; }
        public string RoleName { get; set; }
    }
}
