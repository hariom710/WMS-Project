using System.ComponentModel.DataAnnotations;

namespace WMS.Domain.Models;

public class Client
{
    [Key]
    public int ClientId { get; set; }

    [Required]
    [MaxLength(100)]
    public string ClientName { get; set; }

    public string? ClientAddress { get; set; }

    // The PDF specifies Numeric(10,0), but treating phones as strings in C# prevents leading-zero deletion
    [MaxLength(15)]
    public string? ClientPhoneNumber { get; set; }

    [MaxLength(20)]
    public string? ClientLocation { get; set; }

    public bool Status { get; set; } = true; // Default to Active (1)
}