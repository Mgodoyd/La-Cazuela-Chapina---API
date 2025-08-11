using System.Text.Json;
using Api.DTOs;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("order")]
    [Authorize]
    public class OrderController : BaseController
    {
        private readonly OrderService _service;
        private readonly RedisService _redis;

        public OrderController(OrderService service, RedisService redis)
        {
            _service = service;
            _redis = redis;
        }
        [HttpPost("create")]
        public Task<IActionResult> Create([FromBody] OrderDto dto)
        {
            return ExecuteAsync(async () =>
            {
                var order = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = order.Id }, new { status = "ok", data = order });
            });
        }

        [HttpGet]
        public Task<IActionResult> GetAll() => ExecuteAsync(async () =>
        {
            string cacheKey = "orders_all";
            var cachedOrders = await _redis.GetAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedOrders))
            {
                var ordersFromCache = JsonSerializer.Deserialize<IEnumerable<OrderDto>>(cachedOrders);
                return Ok(new { status = "ok", data = ordersFromCache });
            }

            var orders = await _service.GetAllAsync();
            await _redis.SetAsync(cacheKey, JsonSerializer.Serialize(orders));

            return Ok(new { status = "ok", data = orders });
        });

        [HttpGet("{id}")]
        public Task<IActionResult> GetById(Guid id) => ExecuteAsync(async () =>
        {
            string cacheKey = $"order_{id}";
            var cachedOrder = await _redis.GetAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedOrder))
            {
                var orderFromCache = JsonSerializer.Deserialize<OrderDto>(cachedOrder);
                return Ok(new { status = "ok", data = orderFromCache });
            }

            var order = await _service.GetByIdAsync(id);
            if (order == null)
                return NotFound(new { Error = "Pedido no encontrado." });

            await _redis.SetAsync(cacheKey, JsonSerializer.Serialize(order));

            return Ok(new { status = "ok", data = order });
        });

        [HttpPut("{id}")]
        public Task<IActionResult> Update(Guid id, [FromBody] OrderDto dto)
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
