using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Test.API.Data;
using Test.API.Models;
using Test.API.Services;

namespace Test.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _repo;
        private readonly IMessageSender _sender;

        public UsersController(IUserRepository repo, IMessageSender sender)
        {
            _repo = repo;
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repo.GetUsers();
            _sender.Send("UsersRetrieved");
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);
            bool v = await _repo.UserExistsById(user.Id);
            if (!v)
            {
                return BadRequest("User does not exists!");
            }
            _sender.Send($"UserRetrieved: {user.Id}");
            return Ok(user);
        }

        [HttpPost("add")]
        public async Task<IActionResult> CreateUser(User user)
        {
            if (await _repo.UserExists(user.Name))
            {
                return BadRequest("User already exists!");
            }

            var createdUser = await _repo.CreateUser(user);

            _sender.Send($"UserCreated: {user.Id}");

            return Created($"/{createdUser.Id}", createdUser.Id);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _repo.GetUser(id);
            bool p = await _repo.UserExists(user.Name);
            if (!p)
            {
                return BadRequest("User does not exists!");
            }
            _sender.Send($"UserDeleted: {user.Id}");
            _repo.DeleteUser(user);

            return Ok();

        }
        [HttpPut]
        public async Task<IActionResult> UpdateUser(User user)
        {
            bool v = await _repo.UserExistsById(user.Id);
            if (!v)
            {
                return BadRequest("User does not exists!");
            }

            _repo.UpdateUser(user);
            _sender.Send($"UserUpdated: {user.Id}");
            return Ok();
        }

    }
}