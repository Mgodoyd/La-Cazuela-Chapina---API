namespace Api.Models

{
    public class InventarioItem
    {
        public Guid Id { get; set; }
        public Guid RawMaterialId { get; set; }
        public virtual MateriaPrima RawMaterial { get; set; } = null!;
        public decimal CurrentQuantity { get; set; }
    }

}