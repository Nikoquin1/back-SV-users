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
            Id_card = 1001312,
            Name = "Test User",
            Email = "test123123@example.com",
            Password = "password123"
        };

        try
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok("Usuario agregado correctamente");
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
            {
                // El código 23505 corresponde a violaciones de unicidad en PostgreSQL
                if (pgEx.ConstraintName.Contains("id_card"))
                {
                    return BadRequest("Error: El ID de tarjeta ya está en uso.");
                }
                if (pgEx.ConstraintName.Contains("email"))
                {
                    return BadRequest("Error: El correo electrónico ya está en uso.");
                }
            }
            Console.WriteLine($"Error: {ex.Message}");
            return StatusCode(500, $"Error al guardar el usuario: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return StatusCode(500, "Error al guardar el usuario.");
        }
    }



}


