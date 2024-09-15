using Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace back_SV_users
{
    public class Entrepreneurship
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")] 
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        [Column("id_user")] 
        public int Id_user { get; set; }

        /*
        [Required]
        [ForeignKey("Plan")]
        [Column("id_plan")] 
        public int Id_plan { get; set; }
        */

        [Required]
        [Column("name")]
        public required string Name { get; set; }

        [Column("logo")]
        public required string Logo { get; set; }

        [Required]
        [Column("nit")]
        public int NIT { get; set; }

        [Column("description")]
        public required string Description { get; set; }

        public required User User { get; set; }
    }
}
