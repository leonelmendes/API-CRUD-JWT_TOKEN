using System.Security.Claims;
using System.Text.RegularExpressions;
using Api.Model;
using Api.Repository.Authenticate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controller
{
    [ApiController]
    [Route("controller")]
    public class AuthenticateController : ControllerBase
    {
        private readonly IAuthenticate_Repository _repository;

        public AuthenticateController(IAuthenticate_Repository repository)
        {
            _repository = repository;
        }

        [HttpPost("Authenticate_Login")]
        [Produces("application/json")]
        public async Task<IActionResult> Authenticate_Login([FromBody] Login_RequestModel login_Request)
        {
            User_Model user = new User_Model();

            Regex defaultEmail = new Regex(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}");
            Regex defaultPhone = new Regex(@"[9][0-9]{8}");

            if(defaultEmail.IsMatch(login_Request.Input))
            {
                user = new User_Model()
                {
                    Email = login_Request.Input,
                    Phone = 0,
                    Password = login_Request.Password
                };
            }
            else if(defaultEmail.IsMatch(login_Request.Input))
            {
                user = new User_Model()
                {
                    Phone = Convert.ToInt32(login_Request.Input),
                    Email = null,
                    Password = login_Request.Password
                };
            }
            else
            {
                user = new User_Model()
                {
                    Email = string.Empty,
                    Phone = 0,
                    Password = string.Empty
                };
            }

            var response = await _repository.Authenticate_Login(user);

            if(response == null)
                return Unauthorized();

            return Ok(response);
        }

        [HttpGet("Get_user_Token")]
        [Authorize]
        [Produces("application/json")]
        public async Task<IActionResult> Get_user_Token()
        {
            var email = User.FindFirst(ClaimTypes.Email);
            var username = email.Value;

            var user = new User_Model()
            {
                Email = username
            };

            string response = await _repository.User_Data(user);
            return response.Contains("incorret") ? NotFound(response) : Ok(user);
        }
    }
}