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
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;


[Route("api/[controller]")]
[AllowAnonymous]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly DatabaseContext _context;
    private readonly Utilities _utilities;

    public UsersController(DatabaseContext context, Utilities utilities)
    {
        _context = context;
        _utilities = utilities;
    }

    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register([FromBody] UserDTO user)
    {
        if (user == null)
        {
            return BadRequest("User data is required.");
        }

        // Buscar el RoleId basado en el RoleName proporcionado
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
            RoleId = role.Id  // Asignar el RoleId encontrado
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
            // Usuario no encontrado o credenciales incorrectas
            return StatusCode(StatusCodes.Status200OK, new { isSuccess = false, token = "" });
        }
        else
        {
            // Usuario encontrado, generamos el token
            var token = _utilities.generateJWT(userFind);
            return StatusCode(StatusCodes.Status200OK, new { isSuccess = true, token = token });
        }
    }


    // Endpoint para listar todos los usuarios
    [HttpGet]
    public IActionResult GetUsers()
    {
        // Filtra los usuarios asegurando que los campos Name y Email no sean nulos, vacíos ni compuestos solo de espacios
        var users = _context.Users
            .Where(u => !string.IsNullOrEmpty(u.Name) && !string.IsNullOrWhiteSpace(u.Name) &&
                        !string.IsNullOrEmpty(u.Email) && !string.IsNullOrWhiteSpace(u.Email))
            .ToList();  // Trae los resultados después de aplicar los filtros

        return Ok(users);
    }

    // Endpoint para agregar un usuario de prueba
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
                // Manejo de excepciones específicas de PostgreSQL para duplicados
                if (pgEx.ConstraintName.Contains("id_card"))
                {
                    return BadRequest("Error: The card ID is already in use.");
                }
                if (pgEx.ConstraintName.Contains("email"))
                {
                    return BadRequest("Error: The e-mail is already in use.");
                }
            }
            // Log del error para revisión
            Console.WriteLine($"Error: {ex.Message}");
            return StatusCode(500, $"Error saving the user: {ex.Message}");
        }
        catch (Exception ex)
        {
            // Manejo general de excepciones
            Console.WriteLine($"Error: {ex.Message}");
            return StatusCode(500, "Error saving the user.");
        }
    }
}
