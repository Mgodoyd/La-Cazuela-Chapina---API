using System.Text.Json;
using Api.DTOs;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("supplier")]
    [Authorize(Roles = "Admin")]
    public class SupplierController : BaseController
    {
        private readonly SupplierService _service;
        private readonly RedisService _redis;

        public SupplierController(SupplierService service, RedisService redis)
        {
            _service = service;
            _redis = redis;
        }

        [HttpPost("create")]
        public Task<IActionResult> Register([FromBody] SupplierDto dto) => ExecuteAsync(async () =>
        {
            var supplier = await _service.RegisterAsync(dto);
            var response = new
            {
                status = "ok",
                data = new
                {
                    supplier.Id,
                    supplier.Name,
                    supplier.Contact,
                    supplier.Phone
                }
            };
            return CreatedAtAction(nameof(GetById), new { id = supplier.Id }, response);
        });

        [HttpGet]
        public Task<IActionResult> GetAll() => ExecuteAsync(async () =>
        {
            string cacheKey = "suppliers_all";
            var cachedSuppliers = await _redis.GetAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedSuppliers))
            {
                var suppliersFromCache = JsonSerializer.Deserialize<IEnumerable<object>>(cachedSuppliers);
                return Ok(new { status = "ok", data = suppliersFromCache });
            }

            var suppliers = await _service.GetAllAsync();
            var projectedSuppliers = suppliers.Select(s => new
            {
                s.Id,
                s.Name,
                s.Contact,
                s.Phone
            });

            await _redis.SetAsync(cacheKey, JsonSerializer.Serialize(projectedSuppliers));

            return Ok(new { status = "ok", data = projectedSuppliers });
        });

        [HttpGet("{id}")]
        public Task<IActionResult> GetById(Guid id) => ExecuteAsync(async () =>
        {
            string cacheKey = $"supplier_{id}";
            var cachedSupplier = await _redis.GetAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedSupplier))
            {
                var supplierFromCache = JsonSerializer.Deserialize<object>(cachedSupplier);
                return Ok(new { status = "ok", data = supplierFromCache });
            }

            var supplier = await _service.GetByIdAsync(id);
            if (supplier == null)
                return NotFound(new { Error = "Proveedor no encontrado." });

            var projectedSupplier = new
            {
                supplier.Id,
                supplier.Name,
                supplier.Contact,
                supplier.Phone
            };

            await _redis.SetAsync(cacheKey, JsonSerializer.Serialize(projectedSupplier));

            return Ok(new { status = "ok", data = projectedSupplier });
        });


        [HttpPut("{id}")]
        public Task<IActionResult> Update(Guid id, [FromBody] SupplierDto dto) => ExecuteAsync(async () =>
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
