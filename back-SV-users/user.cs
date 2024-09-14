namespace back_SV_users // Debe coincidir exactamente con el namespace en DatabaseContext
{
    public class User 
    {
        public required int Id_card { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
