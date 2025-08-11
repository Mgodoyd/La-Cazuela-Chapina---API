using Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Api.Services
{
    public class BusinessContextProvider
    {
        private readonly ApplicationDbContext _db;
        private readonly EmbeddingService _embeddings;

        public BusinessContextProvider(ApplicationDbContext db, EmbeddingService embeddings)
        {
            _db = db;
            _embeddings = embeddings;
        }

        public async Task<string> RetrieveContextAsync(string message, CancellationToken ct = default)
        {
            // Obtener embedding de la consulta
            var queryEmbedding = await _embeddings.CreateEmbeddingAsync(message);

            // Obtener chunks con embedding no nulo
            var knowledgeChunks = await _db.Knowledge
                .Where(k => !string.IsNullOrEmpty(k.Embedding))
                .AsNoTracking()
                .ToListAsync(ct);

            // Deserializar embeddings y evitar excepciones
            var chunkEmbeddings = knowledgeChunks
                .Select(k =>
                {
                    try
                    {
                        var embedding = JsonSerializer.Deserialize<float[]>(k.Embedding!);
                        return embedding != null ? new { Chunk = k, Embedding = embedding } : null;
                    }
                    catch
                    {
                        return null;
                    }
                })
                .Where(x => x != null)
                .ToList()!;

            // Calcular similitud coseno y ordenar para obtener los top 5 más relevantes
            var similarities = chunkEmbeddings
                .Select(c => new
                {
                    c!.Chunk,
                    Similarity = CosineSimilarity(queryEmbedding, c.Embedding)
                })
                .OrderByDescending(x => x.Similarity)
                .Take(5)
                .ToList();

            Console.WriteLine("Similitudes encontradas:");
            foreach (var sim in similarities)
            {
                Console.WriteLine($"Texto: {sim.Chunk.Text} - Similaridad: {sim.Similarity:F4}");
            }

            // Concatenar contexto relevante separado por "---"
            var context = string.Join("\n---\n", similarities.Select(x => x.Chunk.Text));

            Console.WriteLine($"Contexto a enviar al chat:\n{context}");

            return context;
        }

        private static double CosineSimilarity(IReadOnlyList<float> a, IReadOnlyList<float> b)
        {
            var len = Math.Min(a.Count, b.Count);
            double dot = 0, na = 0, nb = 0;
            for (int i = 0; i < len; i++)
            {
                dot += a[i] * b[i];
                na += a[i] * a[i];
                nb += b[i] * b[i];
            }
            var denom = Math.Sqrt(na) * Math.Sqrt(nb);
            return denom == 0 ? 0 : dot / denom;
        }
    }
}
