using Api.DTOs;
using Api.Models;
using Api.Interface;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Api.Services
{
    public class ProductService
    {
        private readonly IRepository<ProductoBase> _productRepo;
        private readonly IRepository<Tamal> _tamalRepo;
        private readonly IRepository<Bebida> _beverageRepo;

        public ProductService(
            IRepository<ProductoBase> productRepo,
            IRepository<Tamal> tamalRepo,
            IRepository<Bebida> beverageRepo)
        {
            _productRepo = productRepo;
            _tamalRepo = tamalRepo;
            _beverageRepo = beverageRepo;
        }

        // Productos base
        public async Task<IEnumerable<ProductoBase>> GetAllAsync()
        {
            return await _productRepo.Query().ToListAsync();
        }

        public async Task<ProductoBase?> GetByIdAsync(Guid id)
        {
            return await _productRepo.GetByIdAsync(id);
        }

        public async Task<ProductoBase> CreateAsync(ProductBaseDto dto)
        {
            if (dto.Price <= 0)
                throw new ValidationException("El precio debe ser mayor a 0.");
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ValidationException("El nombre es requerido.");

            var product = new ProductoBase
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Active = dto.Active,
                Stock = dto.Stock,
                CreatedAt = DateTime.UtcNow
            };

            await _productRepo.AddAsync(product);
            await _productRepo.SaveChangesAsync();

            return product;
        }

        public async Task<Tamal> CreateTamalAsync(TamalDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Filling))
                throw new ValidationException("El relleno es requerido.");
            if (string.IsNullOrWhiteSpace(dto.Wrapper))
                throw new ValidationException("La envoltura es requerida.");

            var tamal = new Tamal
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Active = dto.Active,
                Stock = dto.Stock,
                CreatedAt = DateTime.UtcNow,
                DoughType = dto.DoughType,
                Filling = dto.Filling,
                Wrapper = dto.Wrapper,
                SpiceLevel = dto.SpiceLevel
            };

            await _tamalRepo.AddAsync(tamal);
            await _tamalRepo.SaveChangesAsync();

            return tamal;
        }

        public async Task<Bebida> CreateBeverageAsync(BeverageDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Sweetener))
                throw new ValidationException("El endulzante es requerido.");
            if (string.IsNullOrWhiteSpace(dto.Size))
                dto.Size = "12oz"; 
            if (dto.Price <= 0)
                throw new ValidationException("El precio debe ser mayor a 0.");
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ValidationException("El nombre es requerido.");

            var beverage = new Bebida
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Active = dto.Active,
                Stock = dto.Stock,
                CreatedAt = DateTime.UtcNow,
                Type = dto.Type,
                Sweetener = dto.Sweetener,
                Topping = dto.Topping,
                Size = dto.Size
            };

            await _beverageRepo.AddAsync(beverage);
            await _beverageRepo.SaveChangesAsync();

            return beverage;
        }

        public async Task UpdateAsync(Guid id, ProductBaseDto dto)
        {
            var product = await _productRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Producto no encontrado.");

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Active = dto.Active;
            product.Stock = dto.Stock;

            _productRepo.Update(product);
            await _productRepo.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var product = await _productRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Producto no encontrado.");
                
            _productRepo.Delete(product);
            await _productRepo.SaveChangesAsync();
        }
    }
}
