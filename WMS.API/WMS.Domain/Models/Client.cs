using System.ComponentModel.DataAnnotations;

namespace WMS.Domain.Models;

public class Client : BaseEntity
{
    [Key]
    public int ClientId { get; set; }

    [Required(ErrorMessage = "Client name is required.")]
    [MaxLength(100, ErrorMessage = "Client name cannot exceed 100 characters.")]
    [MinLength(3, ErrorMessage = "Client name must be at least 3 characters.")]
    public string ClientName { get; set; }

    [Required(ErrorMessage = "Address is required.")]
    public string? ClientAddress { get; set; }

    [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
    [MaxLength(15)]
    public string? ClientPhoneNumber { get; set; }

    [Required(ErrorMessage = "Location is required.")]
    [MaxLength(20)]
    public string? ClientLocation { get; set; }

    public bool Status { get; set; } = true;
}
