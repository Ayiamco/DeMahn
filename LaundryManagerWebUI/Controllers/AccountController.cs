using LaundryManagerAPIDomain.Entities;
using LaundryManagerWebUI.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryManagerAPIDomain.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LaundryManagerWebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<ApplicationUser> _roleManager;
        private SignInManager<ApplicationUser> _signManager;
        public AccountController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signManager = signInManager;
        }


        [HttpPost("laundry/new")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid) return BadRequest();

            var user = new ApplicationUser
            {
                UserName = model.Username, Email = model.Username,
                Profile = new UserProfile()
                {
                    Name = model.OwnerName,CreatedAt = DateTime.Now,UpdatedAt = DateTime.Now,
                    Laundry = new Laundry
                    {
                        Name = model.LaundryName,CreatedAt = DateTime.Now,UpdatedAt = DateTime.Now,
                        Address = new Location
                        {
                            State = model.Address.State,LGA = model.Address.State,Country = "Nigeria",
                            Street = model.Address.Street
                        }

                    }
                }
            };
            var result = await _userManager.CreateAsync(user, model.Password); ;

            if (result.Succeeded) 
            {
                await _userManager.AddToRoleAsync(user,RoleNames.Owner);
                string id = user.Id;
                return Ok();
            }
           

            return  StatusCode(500);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login ([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid) return BadRequest();

            var result =await _signManager.PasswordSignInAsync(model.Username, model.Password,false,false);
            if (result.Succeeded) return Ok();

            return BadRequest();

        }
    }
}
