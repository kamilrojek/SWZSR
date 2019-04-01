using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SWZSR.ViewModels
{
    public class LoginRegisterViewModel
    {
        public LoginViewModel LoginViewModel { get; set; }
        public RegisterViewModel RegisterViewModel { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Wprowadź poprawny adres email")]
        [Display(Name = "Adres email")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Musisz wprowadzić hasło")]
        [Display(Name = "Hasło")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Należy podać imię")]
        [Display(Name = "Imię")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Należy podać nazwisko")]
        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Należy podać email")]
        [Display(Name = "Adres email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Należy podać numer telefonu")]
        [Display(Name = "Numer telefonu")]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Należy wprowadzić hasło")]
        [StringLength(100, ErrorMessage = "Hasło nie może być krótsze niż {2} znaków", MinimumLength = 7)]
        [Display(Name = "Hasło")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Należy wprowadzić hasło")]
        [Display(Name = "Powtórz hasło")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Hasła różnią się")]
        public string ConfirmPassword { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "Należy zaakceptować postanowienia regulaminu")]
        public bool RegAccept { get; set; }
    }
}
