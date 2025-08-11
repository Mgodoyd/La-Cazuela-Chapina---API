using System.ComponentModel.DataAnnotations;

namespace Api.DTOs

{
    public class SaleDto
    {
        // public Guid Id { get; set; }
        public DateTime Date { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El total debe ser mayor a 0.")]
        public decimal Total { get; set; }
        public List<SaleDetailDto> Items { get; set; } = new List<SaleDetailDto>();
    }
}