namespace Api.DTOs

{
    public class TamalDto : ProductBaseDto
    {
        public string DoughType { get; set; } = string.Empty;
        public string Filling { get; set; } = string.Empty;
        public string Wrapper { get; set; } = string.Empty;
        public string SpiceLevel { get; set; } = string.Empty;
    }
}