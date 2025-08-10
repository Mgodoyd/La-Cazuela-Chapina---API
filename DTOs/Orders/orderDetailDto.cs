using System.ComponentModel.DataAnnotations;

namespace Api.DTOs

{
     public class OrderDetailDto
    {
        // public Guid ProductId { get; set; }
        [Required]
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }  
    }

}