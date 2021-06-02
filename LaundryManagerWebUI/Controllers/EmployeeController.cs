using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryManagerWebUI.Interfaces;
using LaundryManagerWebUI.Dtos;
using LaundryManagerWebUI.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LaundryManagerWebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            var resp =await employeeService.AddEmployeeToTransit(model);

            if(resp.Result == AppServiceResult.Succeeded)   return Ok( resp.Data);

            return StatusCode(500);
        }
    }
}
