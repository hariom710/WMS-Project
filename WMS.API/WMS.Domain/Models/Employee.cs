using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Domain.Models
{
    public class Employee : BaseEntity
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        [RegularExpression(@"^[A-Za-z ]+$", ErrorMessage = "First name must contain only alphabets.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        [RegularExpression(@"^[A-Za-z ]+$", ErrorMessage = "Last name must contain only alphabets.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [MaxLength(80, ErrorMessage = "Email cannot exceed 80 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
        public string PhoneNumber { get; set; }

        [MaxLength(1), RegularExpression("^[MF]$", ErrorMessage = "Gender must be M or F")]
        public string? Gender { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Date of joining is required.")]
        public DateTime DateOfJoining { get; set; }

        [ForeignKey("Department")]
        [Required(ErrorMessage = "Department is required.")]
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        [ForeignKey("Role")]
        [Required(ErrorMessage = "Role is required.")]
        public int RoleId { get; set; }
        public Role? Role { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Active";
    }
}
