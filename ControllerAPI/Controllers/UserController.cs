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
    // GET: api/User/{id}
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

    // Create new user
    // POST: api/User
    [HttpPost]
    public ActionResult<User> CreateUser([FromBody] User user)
    {
        users.Add(user);

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    // Update user by id
    // PUT: api/User/{id}
    [HttpPut("{id}")]
    public IActionResult UpdateUser(int id, [FromBody] User user)
    {
        // Validate user id
        if (id != user.Id)
        {
            return BadRequest();
        }

        var userToUpdate = users.Find(u => u.Id == id);

        if (userToUpdate == null)
        {
            return NotFound();
        }

        userToUpdate.Username = user.Username;
        userToUpdate.Email = user.Email;
        userToUpdate.Fullname = user.Fullname;

        return Ok(userToUpdate);
    }

    // Delete user by id
    // DELETE: api/User/{id}
    [HttpDelete("{id}")]
    public ActionResult DeleteUser(int id)
    {
        var userToDelete = users.Find(u => u.Id == id);

        if (userToDelete == null)
        {
            return NotFound();
        }

        users.Remove(userToDelete);

        return NoContent();
    }
}