using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs
{
    public class ClientDto
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string? ClientAddress { get; set; }
        public string? ClientPhoneNumber { get; set; }
        public string? ClientLocation { get; set; }
        public bool Status { get; set; }
    }

    public class CreateClientDto
    {
        [Required(ErrorMessage = "Client name is required.")]
        [MaxLength(100, ErrorMessage = "Client name cannot exceed 100 characters.")]
        [MinLength(3, ErrorMessage = "Client name must be at least 3 characters.")]
        public string ClientName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required.")]
        public string? ClientAddress { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
        public string? ClientPhoneNumber { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        public string? ClientLocation { get; set; }

        public bool Status { get; set; } = true;
    }

    public class UpdateClientDto
    {
        [Required(ErrorMessage = "Client name is required.")]
        [MaxLength(100, ErrorMessage = "Client name cannot exceed 100 characters.")]
        [MinLength(3, ErrorMessage = "Client name must be at least 3 characters.")]
        public string ClientName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required.")]
        public string? ClientAddress { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
        public string? ClientPhoneNumber { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        public string? ClientLocation { get; set; }

        public bool Status { get; set; } = true;
    }
}
