using Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace back_SV_users
{
    public class Client
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")] 
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")] 
        [Column("id_user")]
        public int Id_user { get; set; }

        [Required]
        [Column("id_card")] 
        public int Id_card { get; set; }

        public User User { get; set; }
    }
}
