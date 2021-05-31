using LaundryManagerAPIDomain.Contracts;
using LaundryManagerAPIDomain.Services.EmailService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LaundryManagerWebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class LaundryController : ControllerBase
    {
        private IEmailSender sender;
        public LaundryController(IEmailSender sender)
        {
            this.sender = sender;
        }
        // GET: api/<LaundryController>
        [HttpGet("sendmessage")]
        public IEnumerable<string> Get()
        {
            sender.SendEmail(new Message(new List<string> { "ayiamco@gmail.com" }, "test", "hello world"));

            return new string[] { "value1", "value2" };
        }

        // GET api/<LaundryController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<LaundryController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<LaundryController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<LaundryController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
