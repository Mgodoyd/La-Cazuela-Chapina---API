using System.Text.Json;
using Api.DTOs;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("combos")]
    [Authorize]
    public class ComboController : BaseController
    {
        private readonly ComboService _service;
        private readonly RedisService _redis;

        public ComboController(ComboService service, RedisService redis)
        {
            _service = service;
            _redis = redis;
        }

        [HttpPost("create")]

        public Task<IActionResult> Create([FromBody] ComboDto dto)
        {
            return ExecuteAsync(async () =>
            {
                var combo = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = combo.Id }, new { status = "ok", data = combo });
            });
        }

        [HttpGet]
        public Task<IActionResult> GetAll() => ExecuteAsync(async () =>
        {
            string cacheKey = "combos_all";
            var cachedCombos = await _redis.GetAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedCombos))
            {
                var combosFromCache = JsonSerializer.Deserialize<IEnumerable<ComboDto>>(cachedCombos);
                return Ok(new { status = "ok", data = combosFromCache });
            }

            var combos = await _service.GetAllAsync();
            await _redis.SetAsync(cacheKey, JsonSerializer.Serialize(combos));

            return Ok(new { status = "ok", data = combos });
        });

        [HttpGet("{id}")]
        public Task<IActionResult> GetById(Guid id) => ExecuteAsync(async () =>
        {
            string cacheKey = $"combo_{id}";
            var cachedCombo = await _redis.GetAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedCombo))
            {
                var comboFromCache = JsonSerializer.Deserialize<ComboDto>(cachedCombo);
                return Ok(new { status = "ok", data = comboFromCache });
            }

            var combo = await _service.GetByIdAsync(id);
            if (combo == null)
                return NotFound(new { Error = "Combo no encontrado." });

            await _redis.SetAsync(cacheKey, JsonSerializer.Serialize(combo));

            return Ok(new { status = "ok", data = combo });
        });

        [HttpPut("{id}")]
        public Task<IActionResult> Update(Guid id, [FromBody] ComboDto dto)
        {
            return ExecuteAsync(async () =>
            {
                await _service.UpdateAsync(id, dto);
                return Ok(new { status = "ok", data = "Actualizado correctamente" });
            });
        }

        [HttpDelete("{id}")]
        public Task<IActionResult> Delete(Guid id)
        {
            return ExecuteAsync(async () =>
            {
                await _service.DeleteAsync(id);
                return Ok(new { status = "ok", data = "Eliminado correctamente" });
            });
        }
    }
}
