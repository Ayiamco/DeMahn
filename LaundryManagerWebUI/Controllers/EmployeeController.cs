using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryManagerWebUI.Interfaces;
using LaundryManagerWebUI.Dtos;
using LaundryManagerWebUI.Services;
using Microsoft.AspNetCore.Authorization;
using LaundryManagerAPIDomain.Services;
using Newtonsoft.Json;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LaundryManagerWebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        [HttpPost("new")]
        public async Task<IActionResult> EmployeeRegister([FromBody] EmployeeInTransitDto model)
        {
            if (!ModelState.IsValid) return BadRequest();
            if (!User.IsInRole(RoleNames.Owner)) return Forbid(JsonConvert.SerializeObject( new 
            {
                errors= new { role= new string[] { "user is not a laundryOwner"} } 
            }));

            var resp =await employeeService.AddEmployeeToTransit(model);
            if(resp.Result == AppServiceResult.Succeeded)   return Ok( resp.Data);
            if(resp.Result == AppServiceResult.Failed)   return BadRequest( resp.Data);

            return StatusCode(500);
        }

        [HttpGet("{id}")]
        public IActionResult GetEmployeeProfile(string id)
        {
            try
            {
                var claims = User.Claims.ToList();
                var claimId = claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault()?.Value;
                if (!User.IsInRole(RoleNames.Owner) && claimId != id) return Unauthorized(new
                {
                    errors = new
                    {
                        role = new string[] { "user is not permitted to view other employees details" }
                    }
                });

                var resp=employeeService.GetEmployee(id);
                if (resp.Result == AppServiceResult.Succeeded) return Ok(resp.Data);
                if (resp.Result == AppServiceResult.Failed) return BadRequest(resp.Data);

                return StatusCode(500);
            }
            catch
            {
                return StatusCode(500);
            }
            
        }

        [HttpPut("{profileId}")]
        public async Task<IActionResult> UpdateProfile(UserProfileDto profile)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest();
                var resp = await employeeService.UpdateEmployeeProfile(profile);
                if (resp.Result == AppServiceResult.Succeeded) return Ok(resp.Data);
                if (resp.Result == AppServiceResult.Failed) return BadRequest(resp.Data);
                return StatusCode(500, resp.Data);
            }
            catch
            {
                //log error
                return StatusCode(500, JsonConvert.SerializeObject(new
                    { errors= new { serverError= new string[] { "something wrong occured"} } }));
            }
            
           
        }
    }
}
