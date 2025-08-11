using System.ComponentModel.DataAnnotations;

namespace Api.DTOs

{
    public class OrderDetailDto
    {
        [Required]
        public Guid ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "El total debe ser mayor a 0.")]
        public decimal UnitPrice { get; set; }
    }

}