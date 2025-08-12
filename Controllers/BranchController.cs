using System.Text.Json;
using Api.DTOs;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("branch")]
    [Authorize(Roles = "Admin")]
    public class BranchController : BaseController
    {
        private readonly BranchService _service;
        private readonly RedisService _redis;

        public BranchController(BranchService service, RedisService redis)
        {
            _service = service;
            _redis = redis;
        }

        [HttpPost("create")]
        public Task<IActionResult> Register([FromBody] BranchDto dto) => ExecuteAsync(async () =>
        {
            var branch = await _service.RegisterAsync(dto);
            await _redis.ClearCacheByPrefixAsync("branches_");
            await _redis.ClearCacheByPrefixAsync("branch_");
            var response = new
            {
                status = "ok",
                data = new
                {
                    branch.Id,
                    branch.Name,
                    branch.Address,
                    branch.Phone
                }
            };
            return CreatedAtAction(nameof(GetById), new { id = branch.Id }, response);
        });

        [HttpGet]
        public Task<IActionResult> GetAll() => ExecuteAsync(async () =>
        {
            const string cacheKey = "branches_all";
            var cached = await _redis.GetAsync(cacheKey);
            if (!string.IsNullOrEmpty(cached))
            {
                var cachedData = JsonSerializer.Deserialize<IEnumerable<BranchDto>>(cached);
                return Ok(new { status = "ok", data = cachedData });
            }

            var data = await _service.GetAllAsync();
            await _redis.SetAsync(cacheKey, JsonSerializer.Serialize(data));
            return Ok(new { status = "ok", data });
        });

        [HttpGet("{id}")]
        public Task<IActionResult> GetById(Guid id) => ExecuteAsync(async () =>
        {
            var cacheKey = $"branch_{id}";
            var cached = await _redis.GetAsync(cacheKey);
            if (!string.IsNullOrEmpty(cached))
            {
                var cachedData = JsonSerializer.Deserialize<BranchDto>(cached);
                return Ok(new { status = "ok", data = cachedData });
            }

            var data = await _service.GetByIdAsync(id);
            if (data == null) return NotFound(new { Error = "Sucursal no encontrada." });

            await _redis.SetAsync(cacheKey, JsonSerializer.Serialize(data));
            return Ok(new { status = "ok", data });
        });

        [HttpPut("{id}")]
        public Task<IActionResult> Update(Guid id, [FromBody] BranchDto dto) => ExecuteAsync(async () =>
        {
            await _service.UpdateAsync(id, dto);
            var response = new { status = "ok", data = "Actualizado correctamente" };
            return Ok(response);
        });

        [HttpDelete("{id}")]
        public Task<IActionResult> Delete(Guid id) => ExecuteAsync(async () =>
        {
            await _service.DeleteAsync(id);
            var response = new { status = "ok", data = "Eliminado correctamente" };
            return Ok(response);
        });
    }
}
