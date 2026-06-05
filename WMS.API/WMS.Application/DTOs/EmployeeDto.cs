using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs
{
    public class EmployeeDto
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime DateOfJoining { get; set; }
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
        public string Status { get; set; } = "Active";
    }

    public class CreateEmployeeDto
    {
        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        [RegularExpression(@"^[A-Za-z ]+$", ErrorMessage = "First name must contain only alphabets.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        [RegularExpression(@"^[A-Za-z ]+$", ErrorMessage = "Last name must contain only alphabets.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [MaxLength(80, ErrorMessage = "Email cannot exceed 80 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
        public string? PhoneNumber { get; set; }

        public string? Gender { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Date of joining is required.")]
        public DateTime DateOfJoining { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        public int RoleId { get; set; }

        public string Status { get; set; } = "Active";
    }

    public class UpdateEmployeeDto
    {
        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        [RegularExpression(@"^[A-Za-z ]+$", ErrorMessage = "First name must contain only alphabets.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        [RegularExpression(@"^[A-Za-z ]+$", ErrorMessage = "Last name must contain only alphabets.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [MaxLength(80, ErrorMessage = "Email cannot exceed 80 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
        public string? PhoneNumber { get; set; }

        public string? Gender { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Date of joining is required.")]
        public DateTime DateOfJoining { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        public int RoleId { get; set; }

        public string Status { get; set; } = "Active";
    }
}
