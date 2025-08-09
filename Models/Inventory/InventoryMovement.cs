namespace Api.Models

{
    public class MovimientoInventario
    {
        public int Id { get; set; }
        public int RawMaterialId { get; set; }
        public MateriaPrima RawMaterial { get; set; } = null!;
        public decimal Quantity { get; set; }
        public string MovementType { get; set; } = string.Empty; // In, Out, Waste
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }

} 