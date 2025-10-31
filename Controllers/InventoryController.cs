using System.Text.Json;
using Api.DTOs;
using Api.Services;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("inventory")]
    [Authorize(Roles = "Admin")]
    public class InventoryController : BaseController
    {
        private readonly InventoryService _service;
        private readonly RedisService _redis;

        public InventoryController(InventoryService service, RedisService redis)
        {
            _service = service;
            _redis = redis;
        }

        [HttpPost("create")]
        public Task<IActionResult> CreateRawMaterial([FromBody] RawMaterialDto dto)
        {
            return ExecuteAsync(async () =>
            {
                var rawMaterial = await _service.CreateRawMaterialAsync(dto);
                await _redis.ClearCacheByPrefixAsync("inventory_");
                await _redis.ClearCacheByPrefixAsync("inventory_item_");
                return CreatedAtAction(nameof(GetInventoryItem), new { rawMaterialId = rawMaterial.Id }, new { status = "ok", data = rawMaterial });
            });
        }

        [HttpGet]
        public Task<IActionResult> GetInventory() => ExecuteAsync(async () =>
        {
            string cacheKey = "inventory_all";
            var cachedInventory = await _redis.GetAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedInventory))
            {
                var inventoryFromCache = JsonSerializer.Deserialize<IEnumerable<InventarioItem>>(cachedInventory);
                return Ok(new { status = "ok", data = inventoryFromCache });
            }

            var inventory = await _service.GetInventoryAsync();
            await _redis.SetAsync(cacheKey, JsonSerializer.Serialize(inventory));

            return Ok(new { status = "ok", data = inventory });
        });

        [HttpGet("{rawMaterialId}")]
        public Task<IActionResult> GetInventoryItem(Guid rawMaterialId) => ExecuteAsync(async () =>
        {
            string cacheKey = $"inventory_item_{rawMaterialId}";
            var cachedItem = await _redis.GetAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedItem))
            {
                var itemFromCache = JsonSerializer.Deserialize<InventarioItem>(cachedItem);
                return Ok(new { status = "ok", data = itemFromCache });
            }

            var item = await _service.GetInventoryItemAsync(rawMaterialId);
            if (item == null)
                return NotFound(new { Error = "Material no encontrado en inventario." });

            await _redis.SetAsync(cacheKey, JsonSerializer.Serialize(item));

            return Ok(new { status = "ok", data = item });
        });

        [HttpPost("{rawMaterialId}/movement")]
        public Task<IActionResult> RegisterMovement(Guid rawMaterialId, [FromBody] InventoryMovementDto dto)
        {
            return ExecuteAsync(async () =>
            {
                var movement = await _service.RegisterMovementAsync(rawMaterialId, dto);
                await _redis.ClearCacheByPrefixAsync("inventory_");
                await _redis.ClearCacheByPrefixAsync("inventory_item_");
                return Ok(new { status = "ok", data = movement });
            });
        }
    }
}
