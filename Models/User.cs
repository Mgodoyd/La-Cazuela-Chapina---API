using System.ComponentModel.DataAnnotations;

namespace Api.Models

{
    public class Usuario
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [MinLength(12)]
        public string PasswordHash { get; set; } = string.Empty;

        [MinLength(12)]
        public string PasswordSalt { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public string Role { get; set; } = "Customer"; // Admin, Seller, etc.
    }

}