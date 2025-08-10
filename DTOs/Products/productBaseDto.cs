using System.ComponentModel.DataAnnotations;

namespace Api.DTOs

{
    public abstract class ProductBaseDto
    {
        // public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public decimal Price { get; set; }
        [Required]
        public bool Active { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}