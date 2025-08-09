namespace Api.Models

{
    public class Combo
    {
        public Guid  Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool Editable { get; set; } = false;
        public virtual ICollection<ProductoCombo> Products { get; set; } = new List<ProductoCombo>();
    }

}