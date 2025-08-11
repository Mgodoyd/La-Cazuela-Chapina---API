namespace Api.Models

{
    public class Bebida : ProductoBase
    {
        public string Type { get; set; } = string.Empty; // atole, pinol, cacao, etc.
        public string Sweetener { get; set; } = string.Empty; // panela, miel, sugar-free
        public string? Topping { get; set; } // marshmallows, cinnamon, cacao
        public string Size { get; set; } = "12oz"; // 12oz or 1L
    }

}