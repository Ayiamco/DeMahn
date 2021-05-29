
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

    public class JWTDto
    {
        public string UserId { get; set; }
        public string JWTSigningKey { get; set; }
        public string RefreshToken { get; set; }
        public string JwtToken { get; set; }
        public string UserRole { get; set; }
        public string UserEmail { get; set; }
    }
}
