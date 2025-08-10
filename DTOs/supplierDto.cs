using System.ComponentModel.DataAnnotations;

namespace Api.DTOs

{
    public class SupplierDto
    {
        // public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Contact { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;
    }
}