using LaundryManagerWebUI.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LaundryManagerWebUI.Infrastructure.CustomValidations
{
    
    public class PasswordCheck : ValidationAttribute
    {
        
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var obj = (LoginDto)validationContext.ObjectInstance;
            Regex rgx = new Regex(@"[A-Z]"); 
            if (!rgx.IsMatch(obj.Password)) return new ValidationResult("Password must contain a capital letter");

            rgx = new Regex(@"[0-9]");
            if (!rgx.IsMatch(obj.Password)) return new ValidationResult("Password must contain a number");

            rgx = new Regex(@"^[a-zA-Z0-9]+$");
            if (rgx.IsMatch(obj.Password)) return new ValidationResult("Password must contain special characters");

            return  ValidationResult.Success;
        }
    }
}
