namespace Api.Models

{
    public class InventarioItem
    {
        public int Id { get; set; }
        public int RawMaterialId { get; set; }
        public MateriaPrima RawMaterial { get; set; } = null!;
        public decimal CurrentQuantity { get; set; }
    }

} 