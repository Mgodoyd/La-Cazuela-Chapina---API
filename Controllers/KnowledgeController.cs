using Api.Data;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("knowledge")]
    [Authorize(Roles = "Admin")]
    public class KnowledgeController : BaseController
    {
        private readonly ApplicationDbContext _db;
        private readonly EmbeddingService _embeddings;
        private readonly string[] _allowedSources = new[] { "ProductBase", "Combo" };

        public KnowledgeController(ApplicationDbContext db, EmbeddingService embeddings)
        {
            _db = db;
            _embeddings = embeddings;
        }

        [HttpPost]
        public Task<IActionResult> Upsert([FromBody] KnowledgeChunkDto dto) => ExecuteAsync(async () =>
        {
            var source = dto.Source ?? "Manual";
            if (!_allowedSources.Contains(source))
                return BadRequest($"Fuente no permitida: {source}");

            var entity = new KnowledgeChunk
            {
                Id = dto.Id ?? Guid.NewGuid(),
                Source = source,
                Text = dto.Text ?? string.Empty,
                CreatedAt = DateTime.UtcNow
            };

            var emb = await _embeddings.CreateEmbeddingAsync(entity.Text);
            entity.Embedding = System.Text.Json.JsonSerializer.Serialize(emb);
            entity.EmbeddedAt = DateTime.UtcNow;

            var existing = await _db.Knowledge.FindAsync(entity.Id);
            if (existing == null)
            {
                await _db.Knowledge.AddAsync(entity);
            }
            else
            {
                existing.Source = entity.Source;
                existing.Text = entity.Text;
                existing.Embedding = entity.Embedding;
                existing.EmbeddedAt = entity.EmbeddedAt;
                _db.Knowledge.Update(existing);
            }

            await _db.SaveChangesAsync();
            return Ok(new { status = "ok", id = entity.Id });
        });

        [HttpGet]
        public Task<IActionResult> List([FromQuery] string? source) => ExecuteAsync(async () =>
        {
            if (source != null && !_allowedSources.Contains(source))
                return Forbid($"Fuente no permitida: {source}");

            var query = _db.Knowledge.AsNoTracking().AsQueryable();

            if (source != null)
                query = query.Where(k => k.Source == source);

            var items = await query
                .OrderByDescending(k => k.CreatedAt)
                .Select(k => new { k.Id, k.Source, k.CreatedAt, k.EmbeddedAt })
                .ToListAsync();

            return Ok(new { status = "ok", data = items });
        });

        public class ImportRequestDto
        {
            public string Source { get; set; } = string.Empty;
        }

        [HttpPost("import")]
        public async Task<IActionResult> Import([FromBody] ImportRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Source))
                return BadRequest("Debe especificar la fuente.");

            if (!_allowedSources.Contains(request.Source))
                return BadRequest($"Fuente no permitida: {request.Source}");

            int importedCount = 0;

            if (request.Source == "ProductBase")
            {
                importedCount = await ImportProductBase(request.Source);
            }
            else if (request.Source == "Combo")
            {
                var combos = await _db.Combo.AsNoTracking().ToListAsync();

                foreach (var combo in combos)
                {
                    var text = combo.Name + (string.IsNullOrEmpty(combo.Description) ? "" : " " + combo.Description);

                    var embedding = await _embeddings.CreateEmbeddingAsync(text);
                    var entity = new KnowledgeChunk
                    {
                        Id = Guid.NewGuid(),
                        Source = request.Source,
                        Text = text,
                        CreatedAt = DateTime.UtcNow,
                        Embedding = System.Text.Json.JsonSerializer.Serialize(embedding),
                        EmbeddedAt = DateTime.UtcNow
                    };
                    await _db.Knowledge.AddAsync(entity);
                    importedCount++;
                }
            }

            await _db.SaveChangesAsync();
            return Ok(new { status = "ok", importedCount });
        }

        private async Task<int> ImportProductBase(string source)
        {
            int importedCount = 0;
            var products = await _db.ProductBase.AsNoTracking().ToListAsync();

            foreach (var product in products)
            {
                var text = product.Name + (string.IsNullOrEmpty(product.Description) ? "" : " " + product.Description);

                var existing = await _db.Knowledge.FindAsync(product.Id);
                if (existing == null)
                {
                    var embedding = await _embeddings.CreateEmbeddingAsync(text);
                    var entity = new KnowledgeChunk
                    {
                        Id = product.Id,
                        Source = source,
                        Text = text,
                        CreatedAt = DateTime.UtcNow,
                        Embedding = System.Text.Json.JsonSerializer.Serialize(embedding),
                        EmbeddedAt = DateTime.UtcNow
                    };
                    await _db.Knowledge.AddAsync(entity);
                    importedCount++;
                }
                else
                {
                    existing.Text = text;
                    var embedding = await _embeddings.CreateEmbeddingAsync(text);
                    existing.Embedding = System.Text.Json.JsonSerializer.Serialize(embedding);
                    existing.EmbeddedAt = DateTime.UtcNow;
                    _db.Knowledge.Update(existing);
                }
            }

            return importedCount;
        }
    }

    public record KnowledgeChunkDto(Guid? Id, string? Source, string? Text);
}
