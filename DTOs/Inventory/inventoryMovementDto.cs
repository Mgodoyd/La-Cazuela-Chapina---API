namespace Api.DTOs

{
    public class InventoryMovementDto
    {
        // public Guid RawMaterialId { get; set; }
        public decimal Quantity { get; set; }
        public string MovementType { get; set; } = string.Empty; // In, Out, Waste
        public DateTime Date { get; set; }
    }
}