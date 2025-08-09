namespace Api.DTOs

{
     public class ComboDto
    {
        // public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool Editable { get; set; }
        public List<ProductComboDto> Products { get; set; } = new();
    }
}