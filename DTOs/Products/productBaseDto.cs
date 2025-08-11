using System.ComponentModel.DataAnnotations;

namespace Api.DTOs

{
    public class ProductBaseDto
    {
        // public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El price debe ser mayor a 0.")]
        public decimal Price { get; set; }
        [Required]
        public bool Active { get; set; }
        public int Stock { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}