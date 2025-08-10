using System.ComponentModel.DataAnnotations;

namespace Api.DTOs

{
    public class InventoryItemDto
    {
        // public Guid  Id { get; set; }
        public Guid RawMaterialId { get; set; }

        [Required]
        public virtual RawMaterialDto RawMaterial { get; set; } = null!;

        [Required]
        public decimal CurrentQuantity { get; set; }
    }
}