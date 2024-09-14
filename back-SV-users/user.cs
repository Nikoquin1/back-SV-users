using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace back_SV_users
{
    public class User
    {
        [Key]
        [Column("id")] // Asegúrate de que el nombre de la columna coincide exactamente
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("id_card")] // Mapea explícitamente a "id_card"
        public required int Id_card { get; set; }

        [Column("name")] // Mapea explícitamente a "name"
        public required string Name { get; set; }

        [Column("email")] // Mapea explícitamente a "email"
        public required string Email { get; set; }

        [Column("password")] // Mapea explícitamente a "password"
        public required string Password { get; set; }
    }
}
