using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.API.Models
{
    public class UserLogin
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } // We will use the Employee's Email for this

        [Required]
        public string PasswordHash { get; set; } // Storing the BCrypt Hash

        [Required]
        public int RoleId { get; set; }

        // The virtual keyword and specific ForeignKey placement helps EF Core map the relationship cleanly
        [ForeignKey("RoleId")]
        public virtual Role? Role { get; set; }

        public DateTime? LastLogin { get; set; } // Updated upon successful authentication
    }
}