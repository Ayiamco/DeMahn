
using LaundryManagerAPIDomain.Entities;
using LaundryManagerWebUI.Infrastructure.CustomValidations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryManagerWebUI.Dtos
{
    public class ConfirmPasswordResetDto:LoginDto
    {
        
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string PasswordToken { get; set; }


    }
    public class EmailDto
    {
        [EmailAddress]
        public string Username { get; set; }
    }
    public class LoginDto:EmailDto
    {
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

    public class EmployeeInTransitDto
    {

        [Required]
        [EmailAddress]
        public string Username { get; set; }

        [Required]
        public Guid LaundryId { get; set; }

        [Required]
        public string LaundryName { get; set; }

    }

    public class NewEmployeeDto:LoginDto
    {
        public Guid Id { get; set; }

        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Name { get; set; }
    }

    public class ConfirmEmailDto
    {
        public string ConfirmationToken { get; set; }
        public string Id { get; set; }
    }

    public class Login2FaDto
    {
        public string username { get; set; }
        public string code { get; set; }
    }
}
