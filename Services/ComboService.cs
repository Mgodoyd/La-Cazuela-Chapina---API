using Api.DTOs;
using Api.Models;
using Api.Interface;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class ComboService
    {
        private readonly IComboRepository _comboRepo;
        private readonly IRepository<ProductoCombo> _productComboRepo;
        private readonly IRepository<ProductoBase> _productRepo;

        public ComboService(
             IComboRepository comboRepo,
            IRepository<ProductoCombo> productComboRepo,
            IRepository<ProductoBase> productRepo)
        {
            _comboRepo = comboRepo;
            _productComboRepo = productComboRepo;
            _productRepo = productRepo;
        }

        public async Task<Combo> CreateAsync(ComboDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ValidationException("El nombre es requerido.");
            if (dto.Price <= 0)
                throw new ValidationException("El precio debe ser mayor a 0.");

            var combo = new Combo
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Editable = dto.Editable
            };

            await _comboRepo.AddAsync(combo);
            await _comboRepo.SaveChangesAsync();

            foreach (var item in dto.Products)
            {
                var productExists = await _productRepo.GetByIdAsync(item.ProductId);
                if (productExists == null)
                    throw new ValidationException($"El producto con ID {item.ProductId} no existe.");

                var productCombo = new ProductoCombo
                {
                    Id = Guid.NewGuid(),
                    ComboId = combo.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };

                await _productComboRepo.AddAsync(productCombo);
            }

            await _productComboRepo.SaveChangesAsync();

            return combo;
        }

        public async Task<IEnumerable<Combo>> GetAllAsync()
        {
            return await _comboRepo.Query()
                .Include(c => c.Products)
                .ThenInclude(pc => pc.Product)
                .ToListAsync();
        }

        public async Task<Combo?> GetByIdAsync(Guid id)
        {
            return await _comboRepo.Query()
                .Include(c => c.Products)
                .ThenInclude(pc => pc.Product)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task UpdateAsync(Guid id, ComboDto dto)
        {
            var combo = await _comboRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Combo no encontrado.");

            combo.Name = dto.Name;
            combo.Description = dto.Description;
            combo.Price = dto.Price;
            combo.Editable = dto.Editable;

            _comboRepo.Update(combo);
            await _comboRepo.SaveChangesAsync();

            // Actualizar productos del combo: para simplicidad eliminamos y agregamos de nuevo
            var existingProducts = _productComboRepo.Query().Where(pc => pc.ComboId == id);
            foreach (var prod in existingProducts)
            {
                _productComboRepo.Delete(prod);
            }
            await _productComboRepo.SaveChangesAsync();

            foreach (var item in dto.Products)
            {
                var productExists = await _productRepo.GetByIdAsync(item.ProductId);
                if (productExists == null)
                    throw new ValidationException($"El producto con ID {item.ProductId} no existe.");

                var productCombo = new ProductoCombo
                {
                    Id = Guid.NewGuid(),
                    ComboId = combo.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };

                await _productComboRepo.AddAsync(productCombo);
            }

            await _productComboRepo.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var combo = await _comboRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Combo no encontrado.");

            var products = _productComboRepo.Query().Where(pc => pc.ComboId == id);
            foreach (var pc in products)
            {
                _productComboRepo.Delete(pc);
            }
            await _productComboRepo.SaveChangesAsync();

            _comboRepo.Delete(combo);
            await _comboRepo.SaveChangesAsync();
        }
    }
}
