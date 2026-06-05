using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Domain.Models
{
    public class Attendance
    {
        [Key]
        public int AttendanceId { get; set; }

        [ForeignKey("Employee")]
        [Required(ErrorMessage = "Employee ID is required.")]
        public int EmpId { get; set; }
        public Employee? Employee { get; set; }

        [Required(ErrorMessage = "Check-in time is required.")]
        public DateTime CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }

        [Range(0, 24, ErrorMessage = "Total hours must be between 0 and 24.")]
        public float? TotalHours { get; set; }

        [Required(ErrorMessage = "Work mode is required.")]
        [MaxLength(20)]
        public string WorkMode { get; set; }

        [Required(ErrorMessage = "Attendance date is required.")]
        public DateTime AttendanceDate { get; set; }
    }
}
