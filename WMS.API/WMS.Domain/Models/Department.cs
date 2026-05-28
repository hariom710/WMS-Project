using System.ComponentModel.DataAnnotations;

namespace WMS.API.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required]
        [MaxLength(100)]
        public string DepartmentName { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}