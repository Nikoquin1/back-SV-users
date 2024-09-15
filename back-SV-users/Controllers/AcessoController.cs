using Microsoft.AspNetCore.Mvc;
using back_SV_users.Data;
using back_SV_users;
using System.Linq;
using Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly DatabaseContext _context;

    public UsersController(DatabaseContext context)
    {
        _context = context;
    }

    // Endpoint to list all users
    [HttpGet]
    public IActionResult GetUsers()
    {
        // Filtra los usuarios asegurando que los campos Name y Email no sean nulos, vacíos ni compuestos solo de espacios
        var users = _context.Users;
        return Ok(users);
    }



    // Endpoint to add a test user



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


