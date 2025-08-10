using System.ComponentModel.DataAnnotations;

namespace Api.DTOs

{
    public class UserDto
    {
        // public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Customer";
    }
}