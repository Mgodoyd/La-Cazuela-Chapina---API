using System.ComponentModel.DataAnnotations;

namespace Api.DTOs

{
    public class BranchDto
    {
        // public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;
        [Required]
        [RegularExpression(@"^\+?\d{8,11}$", ErrorMessage = "El teléfono debe contener entre 8 y 11 dígitos y puede comenzar con +.")]
        public string Phone { get; set; } = string.Empty;
    }
}