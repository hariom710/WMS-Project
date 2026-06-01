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
        public string ClientName { get; set; } = string.Empty;
        public string? ClientAddress { get; set; }
        public string? ClientPhoneNumber { get; set; }
        public string? ClientLocation { get; set; }
        public bool Status { get; set; } = true;
    }

    public class UpdateClientDto
    {
        public string ClientName { get; set; } = string.Empty;
        public string? ClientAddress { get; set; }
        public string? ClientPhoneNumber { get; set; }
        public string? ClientLocation { get; set; }
        public bool Status { get; set; } = true;
    }
}
