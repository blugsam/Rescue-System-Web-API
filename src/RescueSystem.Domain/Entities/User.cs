using RescueSystem.Domain.Entities.Bracelets;

namespace RescueSystem.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public required string FullName { get; set; }
        public DateOnly DateOfBirth { get; set; }

        public Bracelet? Bracelet { get; set; }
    }
}