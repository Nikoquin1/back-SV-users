using Microsoft.AspNetCore.Mvc;
using back_SV_users.Data;
using back_SV_users;
using System.Linq;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly DatabaseContext _context;

    public UsersController(DatabaseContext context)
    {
        _context = context;
    }

    // Endpoint para listar todos los usuarios
    [HttpGet]
    public IActionResult GetUsers()
    {
        var users = _context.Users.ToList();
        return Ok(users);
    }

    // Endpoint para agregar un usuario de prueba

 
    [HttpPost]
    public IActionResult AddUser()
    {
        var user = new User
        {
            Id_card = 100,
            Name = "Test User",
            Email = "test@example.com",
            Password = "password123"
        };

        try
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok("Usuario agregado correctamente");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return StatusCode(500, "Error al guardar el usuario.");
        }
    }

}


