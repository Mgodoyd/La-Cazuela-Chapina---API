using System.ComponentModel.DataAnnotations;

namespace Api.DTOs

{
    public class SaleDetailDto
    {
        // public Guid Id { get; set; }
        public Guid SaleId { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "El precio unitario debe ser mayor a 0.")]
        public decimal UnitPrice { get; set; }
    }
}