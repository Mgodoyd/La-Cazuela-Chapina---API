namespace Api.Models

{
    public class MateriaPrima
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty; // kg, liters, etc.
        public decimal MinStock { get; set; }
    }

}