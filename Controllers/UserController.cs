using System.Text.RegularExpressions;
using Api.Model;
using Api.Repository.User;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controller
{
    [ApiController]
    [Route("controller")]
    public class UserController : ControllerBase
    {
        private readonly IUser_Repository _repository;
        public UserController(IUser_Repository repository)
        {
            _repository = repository;
        }

        [HttpPost("Create_User")]
        [Produces("application/json")]
        public async Task<IActionResult> Create_User(User_Model user)
        {
            string response = await _repository.Create_User(user);
            return response.Contains("completed") ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("Delete_User")]
        [Produces("application/json")]
        public async Task<IActionResult> Delete_User(string input)
        {
            User_Model user = new User_Model();

            Regex defaultEmail = new Regex(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}");
            Regex defaultPhone = new Regex(@"[9][0-9]{8}");

            if(defaultEmail.IsMatch(input))
            {
                user = new User_Model()
                {
                    Email = input,
                    Phone = 0,
                };
            }
            else if(defaultEmail.IsMatch(input))
            {
                user = new User_Model()
                {
                    Phone = Convert.ToInt32(input),
                    Email = null,
                };
            }
            else
            {
                user = new User_Model()
                {
                    Email = string.Empty,
                    Phone = 0,
                };
            }

            string response = await _repository.Delete_User(user);
            return response.Contains("successfully") ? Ok(response) : BadRequest(response);
        }

        [HttpPut("Update_User")]
        [Produces("application/json")]
        public async Task<IActionResult> Update_User(User_Model user)
        {
            string response = await _repository.Update_User(user);
            return response.Contains("successfully") ? Ok(response) : BadRequest(response);
        }
    }
}