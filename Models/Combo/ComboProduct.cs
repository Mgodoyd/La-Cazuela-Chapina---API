namespace Api.Models

{
    public class ProductoCombo
    {
        public Guid Id { get; set; }
        public Guid ComboId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }

        public virtual ProductoBase? Product { get; set; }
    }

}