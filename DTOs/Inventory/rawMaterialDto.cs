using System.ComponentModel.DataAnnotations;

namespace Api.DTOs

{
    public class RawMaterialDto
    {
        // public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Unit { get; set; } = string.Empty; // kg, liters, etc.

        [Required]
        public decimal MinStock { get; set; }
    }
}