using System.ComponentModel.DataAnnotations;

namespace Api.DTOs

{
    public class ComboDto
    {
        // public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0.")]
        public decimal Price { get; set; }
        public bool Editable { get; set; }
        public List<ProductComboDto> Products { get; set; } = new();
    }
}