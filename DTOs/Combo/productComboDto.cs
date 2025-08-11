using System.ComponentModel.DataAnnotations;

namespace Api.DTOs

{
    public class ProductComboDto
    {
        // public Guid Id { get; set; }
        public Guid ComboId { get; set; }
        public Guid ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }
    }

}