using Api.DTOs;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Api.Controllers
{
    [ApiController]
    [Route("product")]
    public class ProductController : BaseController
    {
        private readonly ProductService _service;
        private readonly RedisService _redis;

        public ProductController(ProductService service, RedisService redis)
        {
            _service = service;
            _redis = redis;
        }

        [HttpGet]
        public Task<IActionResult> GetAll() => ExecuteAsync(async () =>
        {
            string cacheKey = "products_all";
            var cachedProducts = await _redis.GetAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedProducts))
            {
                var productsFromCache = JsonSerializer.Deserialize<IEnumerable<object>>(cachedProducts);
                return Ok(new { status = "ok", data = productsFromCache });
            }

            var products = await _service.GetAllAsync();

            // Opcional: proyectar solo propiedades necesarias
            var projectedProducts = products.Select(p => new
            {
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.Stock,
                p.Active,
                p.CreatedAt
            });

            await _redis.SetAsync(cacheKey, JsonSerializer.Serialize(projectedProducts));

            return Ok(new { status = "ok", data = projectedProducts });
        });

        [HttpGet("{id}")]
        public Task<IActionResult> GetById(Guid id) => ExecuteAsync(async () =>
        {
            string cacheKey = $"product_{id}";
            var cachedProduct = await _redis.GetAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedProduct))
            {
                var productFromCache = JsonSerializer.Deserialize<object>(cachedProduct);
                return Ok(new { status = "ok", data = productFromCache });
            }

            var product = await _service.GetByIdAsync(id);
            if (product == null)
                return NotFound(new { Error = "Producto no encontrado." });

            var projectedProduct = new
            {
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.Stock,
                product.Active,
                product.CreatedAt,
            };

            await _redis.SetAsync(cacheKey, JsonSerializer.Serialize(projectedProduct));

            return Ok(new { status = "ok", data = projectedProduct });
        });


        [HttpPost("create")]
        public Task<IActionResult> Create([FromBody] ProductBaseDto dto) => ExecuteAsync(async () =>
        {
            var product = await _service.CreateAsync(dto);
            await _redis.ClearCacheByPrefixAsync("products_");
            await _redis.ClearCacheByPrefixAsync("product_");
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, new { status = "ok", data = product });
        });

        [HttpPost("tamal")]
        public Task<IActionResult> CreateTamal([FromBody] TamalDto dto) => ExecuteAsync(async () =>
        {
            var tamal = await _service.CreateTamalAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = tamal.Id }, new { status = "ok", data = tamal });
        });

        [HttpPost("beverage")]
        public Task<IActionResult> CreateBeverage([FromBody] BeverageDto dto) => ExecuteAsync(async () =>
        {
            var beverage = await _service.CreateBeverageAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = beverage.Id }, new { status = "ok", data = beverage });
        });

        [HttpPut("{id}")]
        public Task<IActionResult> Update(Guid id, [FromBody] ProductBaseDto dto) => ExecuteAsync(async () =>
        {
            await _service.UpdateAsync(id, dto);
            return Ok(new { status = "ok", data = "Producto actualizado correctamente" });
        });

        [HttpDelete("{id}")]
        public Task<IActionResult> Delete(Guid id) => ExecuteAsync(async () =>
        {
            await _service.DeleteAsync(id);
            return Ok(new { status = "ok", data = "Producto eliminado correctamente" });
        });
    }
}
