namespace Api.Models
{
    public class KnowledgeChunk
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Source { get; set; } = string.Empty; // "Products", "Policies"
        public string Text { get; set; } = string.Empty;   // chunk plain text
        public string? Embedding { get; set; }             // JSON array of floats
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? EmbeddedAt { get; set; }
    }
}


