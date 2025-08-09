namespace Api.DTOs

{
      public class VoiceCommandDto
    {
        // public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string TranscribedText { get; set; } = string.Empty;
        public string Intent { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}