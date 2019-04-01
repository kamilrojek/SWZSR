using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SWZSR.ViewModels
{
    public class ManageCredentialsViewModel
    {
        public string UserRole { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class ChangeCredentialsViewModel
    {
        [Phone]
        [Required(ErrorMessage = "Należy podać numer telefonu")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Należy podać imię")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Należy podać nazwisko")]
        public string LastName { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Należy wprowadzić hasło")]
        [Display(Name = "Aktualne hasło")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Należy wprowadzić hasło")]
        [StringLength(100, ErrorMessage = "Hasło nie może być krótsze niż {2} znaków", MinimumLength = 7)]
        [Display(Name = "Nowe hasło")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Należy wprowadzić hasło")]
        [Display(Name = "Powtórz nowe hasło")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Hasła różnią się")]
        public string ConfirmNewPassword { get; set; }
    }
}
