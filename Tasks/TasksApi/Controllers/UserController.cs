using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using TasksApi.Interfaces;
using TasksApi.Model;
using TasksApi.Model.Auth;
using TasksApi.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TasksApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private IAuthService userService;
        private readonly ILogger<UserController> logger;

        public UserController(ILogger<UserController> logger, IAuthService userService)
        {
            this.logger = logger;
            this.userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthRequest user)
        {
            var response = userService.Authenticate(user);
            if (response == null)
            {
                return BadRequest(new { message = "Couldn't authenticate with provided credentials" });
            }
            return Ok(response);
        }




        // GET: api/<UserControllerr>
        [HttpGet]
        public IActionResult GetAll()
        {
            var response = userService.GetAll();
            if (response == null)
            {
                return NotFound(new { message = "Data not found" });
            }
            return Ok((IEnumerable<User>)response);
        }

        // GET api/<UserControllerr>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = userService.GetOne(id);
            if (response == null)
            {
                return NotFound(new { message = "Data not found" });
            }
            return Ok(response);
        }

        // POST api/<UserControllerr>
        [HttpPost]
        public IActionResult Post([FromBody] User user)
        {
            var response = userService.Create(user);
            if (!response)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(response);
        }

        // PUT api/<UserControllerr>/5
        [HttpPatch("{id}")]
        public IActionResult Patch(int id, [FromBody] string value)
        {
            var json = JObject.Parse(value);
            User user = new User()
            {
                FirstName = json["first_name"]?.Value<string>() ?? "",
                LastName = json["last_name"]?.Value<string>() ?? "",
                Email = json["email"]?.Value<string>() ?? "",
                PhoneNumber = json["phone_number"]?.Value<string>() ?? ""
            };

            var response = userService.Update(user);
            if (!response)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            return Ok(response);
        }

        // DELETE api/<UserControllerr>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var response = userService.Delete(id);

            if (!response)
            {
                return NotFound(new { message = "Data not found" });
            }
            return Ok(response);
        }
    }
}
