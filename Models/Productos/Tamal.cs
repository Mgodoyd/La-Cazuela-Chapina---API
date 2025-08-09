namespace Api.Models

{
    public class Tamal : ProductoBase
    {
        public string TipoMasa { get; set; } = string.Empty; // amarillo, blanco, arroz
        public string Relleno { get; set; } = string.Empty;  // cerdo, pollo, vegetariano, etc.
        public string Envoltura { get; set; } = string.Empty; // plátano, tusa
        public string NivelPicante { get; set; } = string.Empty; // sin, suave, chapín
    }

}