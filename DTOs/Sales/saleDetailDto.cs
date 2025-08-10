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
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}