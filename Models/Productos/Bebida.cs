namespace Api.Models

{
    public class Bebida : ProductoBase
    {
        public string Tipo { get; set; } = string.Empty; // atole, pinol, cacao, etc.
        public string Endulzante { get; set; } = string.Empty; // panela, miel, sin azúcar
        public string? Topping { get; set; } // malvaviscos, canela, cacao
        public string Tamaño { get; set; } = "12oz"; // 12oz o 1L
    }

}   