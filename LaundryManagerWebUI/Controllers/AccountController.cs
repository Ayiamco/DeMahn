using LaundryManagerAPIDomain.Entities;
using LaundryManagerWebUI.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryManagerAPIDomain.Services;
using LaundryManagerWebUI.Interfaces;
using LaundryManagerWebUI.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LaundryManagerWebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IAuthService _authService;
        public AccountController(IAuthService _authService)
        {
            this._authService = _authService;
        }


        [HttpPost("laundry/new")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid) return BadRequest();

            var result = await _authService.CreateLaundry(model);
            if (result.Result == AppServiceResult.Succeeded) return Ok(result.Data);

            return StatusCode(500);
        }

        

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid) return BadRequest();

            var result = await _authService.Authenticate(model);
            if (result.Result == AppServiceResult.Succeeded) return Ok(result.Data);
            if (result.Result == AppServiceResult.Failed) return BadRequest(result.Data);
            return StatusCode(500,result.Data);

        }

        [HttpPost("refreshToken")]
        public async Task<ActionResult> RefreshToken([FromBody] JWTDto model)
        {
            var resp=await _authService.RefreshJWtToken(model);

            if(resp.Result== AppServiceResult.Succeeded)  return Ok(resp.Data);

            return BadRequest(resp.Data);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] EmailDto model)
        {
            await _authService.SendResetPasswordLink(model.Username);

            return Ok();
        }

        [HttpPost("confirm-password-reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ConfirmPasswordResetDto model)
        {
           var resp= await _authService.ResetPassword(model);
           if(resp.Result == AppServiceResult.Succeeded) return Ok(resp.Data);
           if (resp.Result == AppServiceResult.Failed) return BadRequest(resp.Data);
           return StatusCode(500);
        }
    }
}
