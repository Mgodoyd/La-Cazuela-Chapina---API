using Api.DTOs;
using Api.Interface;
using Api.Models;
using System.ComponentModel.DataAnnotations;

namespace Api.Services
{
    public class SupplierService
    {
        private readonly IRepository<Proveedor> _repository;

        public SupplierService(IRepository<Proveedor> repository)
        {
            _repository = repository;
        }

        public async Task<Proveedor> RegisterAsync(SupplierDto dto)
        {
            var exists = (await _repository.GetAllAsync())
                .Any(s => s.Name.ToLower() == dto.Name.ToLower());

            if (exists)
                throw new ValidationException("Ya existe un proveedor con este nombre.");

            var supplier = new Proveedor
            {
                Id = Guid.NewGuid(),
                Name = dto.Name.Trim(),
                Contact = dto.Contact.Trim(),
                Phone = dto.Phone.Trim()
            };

            await _repository.AddAsync(supplier);
            await _repository.SaveChangesAsync();

            return supplier;
        }

        public async Task<List<Proveedor>> GetAllAsync()
        {
            return (List<Proveedor>)await _repository.GetAllAsync();
        }

        public async Task<Proveedor?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task UpdateAsync(Guid id, SupplierDto dto)
        {
            var supplier = await _repository.GetByIdAsync(id)
                           ?? throw new KeyNotFoundException("Proveedor no encontrado.");

            supplier.Name = dto.Name.Trim();
            supplier.Contact = dto.Contact.Trim();
            supplier.Phone = dto.Phone.Trim();

            _repository.Update(supplier);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var supplier = await _repository.GetByIdAsync(id)
                           ?? throw new KeyNotFoundException("Proveedor no encontrado.");

            _repository.Delete(supplier);
            await _repository.SaveChangesAsync();
        }
    }
}
