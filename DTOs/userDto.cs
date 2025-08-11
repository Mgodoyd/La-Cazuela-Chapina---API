using System.ComponentModel.DataAnnotations;

namespace Api.DTOs
{
  public class UserDto
  {
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    // Para registrar/login: contraseña en texto plano
    public string? Password { get; set; } = string.Empty;

    // Para cambio de contraseña
    public string? CurrentPassword { get; set; }
    public string? NewPassword { get; set; }

    public string Role { get; set; } = "Customer";

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
  }


}
