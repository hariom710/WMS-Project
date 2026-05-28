using System.ComponentModel.DataAnnotations;

namespace WMS.API.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        [MaxLength(50)]
        public string RoleName { get; set; }

        [MaxLength(150)]
        public string Description { get; set; }
    }
}