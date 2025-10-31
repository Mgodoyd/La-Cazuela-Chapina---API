using System.ComponentModel.DataAnnotations;

namespace Api.DTOs

{
    public class VoiceCommandDto
    {
        // public Guid Id { get; set; }
        public Guid UserId { get; set; }

        [Required]
        public string TranscribedText { get; set; } = string.Empty;

        [Required]
        public string Intent { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}