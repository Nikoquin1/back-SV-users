public class UserCreateDTO
{
    public int Id_card { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    //public int RoleId { get; set; } 
}