namespace RescueSystem.Application.Contracts.Requests
{
    public class BraceletDetailsDto
    {
        public Guid Id { get; set; }
        public string SerialNumber { get; set; } = null!;
        public string Status { get; set; } = null!;
        public UserDetailsDto User { get; set; } = null!;
    }
}
