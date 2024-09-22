using Microsoft.AspNetCore.Mvc;
using back_SV_users.Data;
using back_SV_users;
using System.Linq;
using Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Microsoft.AspNetCore.Authorization;
using Custom;
using DTO;

[Route("/user")]
[AllowAnonymous]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly DatabaseContext _context;
    private readonly Utilities _utilities;
    private readonly EmailService _emailService;

    public UsersController(DatabaseContext context, Utilities utilities, EmailService emailService)
    {
        _context = context;
        _utilities = utilities;
        _emailService = emailService;
    }

    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register([FromBody] UserDTO user)
    {
        if (user == null)
        {
            return BadRequest("User data is required.");
        }

        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == user.RoleName);
        if (role == null)
        {
            return BadRequest("Role does not exist.");
        }

        var modelUser = new User
        {
            Id_card = user.Id_card,
            Name = user.Name,
            Email = user.Email,
            Password = _utilities.encriptarSHA256(user.Password),
            RoleId = role.Id
        };

        try
        {
            await _context.Users.AddAsync(modelUser);
            await _context.SaveChangesAsync();
            return Ok(new { isSuccess = true });
        }
        catch (DbUpdateException dbEx)
        {
            Console.WriteLine($"Database Update Error: {dbEx.InnerException?.Message}");
            return StatusCode(500, $"Database Update Error: {dbEx.InnerException?.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Internal server error: {ex.Message}");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO user)
    {
        if (user == null)
        {
            return BadRequest("User data is required.");
        }

        var userFind = await _context.Users
            .Where(u =>
                u.Email == user.Email &&
                u.Password == _utilities.encriptarSHA256(user.Password)
            ).FirstOrDefaultAsync();

        if (userFind == null)
        {
            return StatusCode(StatusCodes.Status200OK, new { isSuccess = false, token = "" });
        }
        else
        {
            var token = _utilities.generateJWT(userFind);
            return StatusCode(StatusCodes.Status200OK, new { isSuccess = true, token = token });
        }
    }

    [HttpPost("password")]
    public IActionResult ChangePassword([FromBody] PasswordChangeDTO model)
    {
        // Find user by email
        var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        // Verify old password
        if (user.Password != _utilities.encriptarSHA256(model.OldPassword))
        {
            return BadRequest("Old password is incorrect.");
        }

        // Update password
        user.Password = _utilities.encriptarSHA256(model.NewPassword);
        _context.SaveChanges();

        // Send confirmation email
        _emailService.SendPasswordChangedConfirmation(user.Email);

        return Ok("Password updated successfully.");
    }


    [HttpGet]
    public IActionResult GetUsers()
    {
        var users = _context.Users
            .Where(u => !string.IsNullOrEmpty(u.Name) && !string.IsNullOrWhiteSpace(u.Name) &&
                        !string.IsNullOrEmpty(u.Email) && !string.IsNullOrWhiteSpace(u.Email))
            .ToList();

        return Ok(users);
    }

    [HttpPost]
    public IActionResult AddUser()
    {
        var user = new User
        {
            Id_card = 100131288,
            Name = "Test User",
            Email = "test8123123@example.com",
            Password = "password123",
            RoleId = 1
        };

        try
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok("User successfully added");
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
            {
                if (pgEx.ConstraintName.Contains("id_card"))
                {
                    return BadRequest("Error: The card ID is already in use.");
                }
                if (pgEx.ConstraintName.Contains("email"))
                {
                    return BadRequest("Error: The e-mail is already in use.");
                }
            }
            Console.WriteLine($"Error: {ex.Message}");
            return StatusCode(500, $"Error saving the user: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return StatusCode(500, "Error saving the user.");
        }
    }
}
