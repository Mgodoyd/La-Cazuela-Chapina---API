namespace Api.DTOs

{
    public class InventoryItemDto
    {
        // public Guid  Id { get; set; }
        public Guid RawMaterialId { get; set; }
        public virtual RawMaterialDto RawMaterial { get; set; } = null!;
        public decimal CurrentQuantity { get; set; }
    }
}