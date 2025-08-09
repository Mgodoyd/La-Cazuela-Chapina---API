namespace Api.DTOs

{
    public abstract class ProductBaseDto
    {
        // public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}