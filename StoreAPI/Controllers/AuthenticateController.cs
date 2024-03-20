
using System.Security.Claims;
using System.IdentityModel.Token.Jwt;
using Microsoft.AspNetCore.Mvc;
using StoreAPI.Data;
using StoreAPI.Models;
using System.Text;

namespace StoreAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticateController : ControllerBase
{
    private readonly UserManager<IdentityUser> userManager;
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly IConfiguration configuration;
    private readonly ApplicationDbContext context;
    private readonly IWebHostEnvironment env;
    public AuthenticateController(ApplicationDbContext _context, IWebHostEnvironment _env, UserManager<IdentityUser> _userManager, RoleManager<IdentityRole> _roleManager, IConfiguration _configuration)
    {
        context = _context;
        env = _env;

        userManager = _userManager;
        roleManager = _roleManager;
        configuration = _configuration;
    }

    // Register new user
    // POST: api/Authenticate/Register
    [HttpPost]
    [Route("register")]
    public async Task<ActionResult> Register([FromBody] RegisterModel model)
    {
        // check existing user
        var existingUser = await userManager.FindByNameAsync(model.Username);
        if (existingUser != null)
        {
            return StatusCode(
                StatusCodes.Status400BadRequest,
                new Response
                {
                    Status = "Error",
                    Message = "User already exists"
                }
            );
        }

        // create new user
        IdentityUser newUser = new IdentityUser
        {
            UserName = model.Username,
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        // add new user and role
        var result = await userManager.CreateAsync(newUser, model.Password);

        if (!result.Succeeded)
        {
            return StatusCode(
               StatusCodes.Status500InternalServerError,
               new Response
               {
                   Status = "Error",
                   Message = "User creation failed"
               }
           );
        }


        return Ok(new Response
        {
            Status = "Success",
            Message = "User creation success"
        })
    }

    // Login
    // POST: /api/Authenticate/Login
    [HttpPost]
    [Route("login")]
    public async Task<ActionResult> Login([FromBody] LoginModel model)
    {
    }

    // Register for admin
    [HttpPost]
    [Route("register-admin")]
    public async Task<ActionResult> RegisterAdmin([FromBody] RegisterModel model)
    {
        var userExists = await _userManager.FindByNameAsync(model.Username!);
        if (userExists != null)
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new Response
                {
                    Status = "Error",
                    Message = "User already exists!"
                }
            );

        IdentityUser user = new()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username
        };

        var result = await _userManager.CreateAsync(user, model.Password!);

        if (!result.Succeeded)
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new Response
                {
                    Status = "Error",
                    Message = "User creation failed! Please check user details and try again."
                }
            );

        if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
        if (!await _roleManager.RoleExistsAsync(UserRoles.Manager))
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.Manager));
        if (!await _roleManager.RoleExistsAsync(UserRoles.User))
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

        return Ok(new Response { Status = "Success", Message = "User created successfully!" });
    }

    // Method for generating JWT token
    private JwtSecurityToken GenerateJwtToken(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));

        var token = new JwtSecurityToken(
            issuer: configuration["JWT:Issuer"],
            audience: configuration["JWT:Audience"],
            claims: authClaims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );
    }
}