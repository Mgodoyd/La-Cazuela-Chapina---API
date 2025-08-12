using System.Text.Json;
using Api.DTOs;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("sale")]
    [Authorize(Roles = "Admin")]
    public class SaleController : BaseController
    {
        private readonly SaleService _service;
        private readonly RedisService _redis;

        public SaleController(SaleService service, RedisService redis)
        {
            _service = service;
            _redis = redis;
        }

        [HttpPost("create")]
        public Task<IActionResult> Register([FromBody] SaleDto dto) => ExecuteAsync(async () =>
        {
            var sale = await _service.RegisterAsync(dto);
            await _redis.ClearCacheByPrefixAsync("sales_");
            await _redis.ClearCacheByPrefixAsync("sale_");
            var response = new
            {
                status = "ok",
                data = new
                {
                    sale.Id,
                    sale.Date,
                    sale.Total,
                    sale.UserId
                }
            };
            return CreatedAtAction(nameof(GetById), new { id = sale.Id }, response);
        });

        [HttpGet]
        public Task<IActionResult> GetAll() => ExecuteAsync(async () =>
        {
            const string cacheKey = "sales_all";
            var cached = await _redis.GetAsync(cacheKey);

            if (!string.IsNullOrEmpty(cached))
            {
                var cachedResponse = JsonSerializer.Deserialize<object>(cached);
                return Ok(new { status = "ok", data = cachedResponse });
            }

            var sales = await _service.GetAllAsync();

            var responseData = sales.Select(s => new
            {
                s.Id,
                s.Date,
                s.Total,
                User = new { s.User.Id, s.User.Name },
                Items = s.Items.Select(d => new
                {
                    d.ProductId,
                    d.Quantity,
                    d.UnitPrice
                })
            });

            await _redis.SetAsync(cacheKey, JsonSerializer.Serialize(responseData));

            return Ok(new { status = "ok", data = responseData });
        });

        [HttpGet("{id}")]
        public Task<IActionResult> GetById(Guid id) => ExecuteAsync(async () =>
        {
            string cacheKey = $"sale_{id}";
            var cached = await _redis.GetAsync(cacheKey);

            if (!string.IsNullOrEmpty(cached))
            {
                var cachedResponse = JsonSerializer.Deserialize<object>(cached);
                return Ok(new { status = "ok", data = cachedResponse });
            }

            var sale = await _service.GetByIdAsync(id);
            if (sale == null)
                return NotFound(new { Error = "Venta no encontrada." });

            var responseData = new
            {
                sale.Id,
                sale.Date,
                sale.Total,
                User = new { sale.User.Id, sale.User.Name },
                Items = sale.Items.Select(d => new
                {
                    d.ProductId,
                    d.Quantity,
                    d.UnitPrice
                })
            };

            await _redis.SetAsync(cacheKey, JsonSerializer.Serialize(responseData));

            return Ok(new { status = "ok", data = responseData });
        });
    }
}
