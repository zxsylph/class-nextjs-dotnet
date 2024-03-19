using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")] // api/User
public class UserController : ControllerBase
{
    // Mock data for users
    private static readonly List<User> users = new List<User>
        {
            new User {
                Id = 1,
                Username = "john",
                Email = "john@email.com",
                Fullname = "John Doe"
            },
            new User {
                Id = 2,
                Username = "samit",
                Email = "samit@email.com",
                Fullname = "Samit Koyom"
            },
        };

    // GET: api/User
    [HttpGet]
    public ActionResult<IEnumerable<User>> GetUsers()
    {

        return Ok(users);
    }

    // Get user by id
    // GET: api/User/1
    [HttpGet("{id}")]
    public ActionResult<User> GetUser(int id)
    {
        var user = users.Find(u => u.Id == id);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }
}