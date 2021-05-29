
using LaundryManagerAPIDomain.Entities;
using LaundryManagerWebUI.Infrastructure.CustomValidations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryManagerWebUI.Dtos
{
    public class LoginDto
    {
        [EmailAddress]
        public string Username { get; set; }

        [StringLength(30,MinimumLength =8,ErrorMessage ="Password must be at least 8 characters")]
        [PasswordCheck]
        public string Password { get; set; }
    }

    public class RegisterDto : LoginDto
    {
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        public string LaundryName {get;set;}
        public string OwnerName { get; set; }
        public Location Address { get; set; }
    }
}
