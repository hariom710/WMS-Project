using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.API.Models
{
    public class Attendance
    {
        [Key]
        public int AttendanceId { get; set; }

        [ForeignKey("Employee")]
        public int EmpId { get; set; }
        public Employee? Employee { get; set; } // <-- Added the ? here

        [Required]
        public DateTime CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }

        public float? TotalHours { get; set; }

        [MaxLength(20)]
        public string WorkMode { get; set; }

        [Required]
        public DateTime AttendanceDate { get; set; }
    }
}