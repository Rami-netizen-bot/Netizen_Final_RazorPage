using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorDemo.Model
{
    public class AppUsers
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("email")]
        public string? Email { get; set; }

        [Column("gender")]
        public string? Gender { get; set; }

        [Column("role")]
        public string? Role { get; set; }

        [Column("remark")]
        public string? Remark { get; set; }

        [Column("password_hash")]
        public string? PasswordHash { get; set; }
    }
}