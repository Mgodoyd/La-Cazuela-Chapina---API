namespace Api.Models

{
    public class ComandoVoz
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public virtual Usuario User { get; set; } = null!;
        public string TranscribedText { get; set; } = string.Empty;
        public string Intent { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }

}