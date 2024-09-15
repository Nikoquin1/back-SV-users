using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("id_card")]
        public int Id_card { get; set; }

        [Required]
        [Column("name")]
        public string Name { get; set; }

        [Required]
        [Column("email")]
        [EmailAddress] // Validate email format
        public string Email { get; set; }

        [Required]
        [Column("password")]
        public string Password { get; set; }

        // Foreign key to Role
        [Required]
        [ForeignKey("Role")]
        [Column("id_rol")]
        public int RoleId { get; set; }

        // Navigation property to Role
        //public required Role Role { get; set; }
    }
}
