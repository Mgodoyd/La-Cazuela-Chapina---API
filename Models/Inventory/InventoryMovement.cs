namespace Api.Models

{
    public class MovimientoInventario
    {
        public Guid  Id { get; set; }
        public Guid  RawMaterialId { get; set; }
        public virtual MateriaPrima RawMaterial { get; set; } = null!;
        public decimal Quantity { get; set; }
        public string MovementType { get; set; } = string.Empty; // In, Out, Waste
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }

} 