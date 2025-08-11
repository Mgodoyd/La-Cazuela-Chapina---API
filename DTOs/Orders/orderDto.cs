using System.ComponentModel.DataAnnotations;

namespace Api.DTOs

{
    public class OrderDto
    {
        // public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }

        [Required]
        public bool Confirmed { get; set; }
        public ICollection<OrderDetailDto> Items { get; set; } = new List<OrderDetailDto>();
    }
}