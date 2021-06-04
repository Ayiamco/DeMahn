using LaundryManagerAPIDomain.Contracts;
using LaundryManagerAPIDomain.Services.EmailService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using LaundryManagerWebUI.Interfaces;
using LaundryManagerWebUI.Services;
using LaundryManagerWebUI.Dtos;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LaundryManagerWebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LaundryController : ControllerBase
    {
        private readonly ILaundryService _laundryService;
        public LaundryController(ILaundryService  laundryService)
        {
            _laundryService = laundryService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            ServiceResponse resp;
            if (string.IsNullOrEmpty(id))
            {
                var claims = User.Claims.ToList();
                id = claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault()?.Value;
                resp = await _laundryService.GetLaundry(new Guid(id),IsIdentityId:true);
            }
            else resp = await _laundryService.GetLaundry(new Guid(id));

            if (resp.Result == AppServiceResult.Succeeded) return Ok(resp.Data);
            if (resp.Result == AppServiceResult.Failed) return BadRequest(resp.Data);

            return  StatusCode(500,resp.Data);
        }

        
    }
}
