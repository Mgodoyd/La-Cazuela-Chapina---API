using Api.DTOs;
using Api.Interface;
using Api.Models;
using System.ComponentModel.DataAnnotations;

namespace Api.Services
{
    public class BranchService
    {
        private readonly IRepository<Sucursal> _repository;

        public BranchService(IRepository<Sucursal> repository)
        {
            _repository = repository;
        }

        public async Task<Sucursal> RegisterAsync(BranchDto dto)
        {
            var exists = (await _repository.GetAllAsync())
                .Any(b => b.Name.ToLower() == dto.Name.ToLower());

            if (exists)
                throw new ValidationException("Ya existe una sucursal con este nombre.");

            var branch = new Sucursal
            {
                Id = Guid.NewGuid(),
                Name = dto.Name.Trim(),
                Address = dto.Address.Trim(),
                Phone = dto.Phone.Trim()
            };

            await _repository.AddAsync(branch);
            await _repository.SaveChangesAsync();

            return branch;
        }

        public async Task<List<Sucursal>> GetAllAsync()
        {
            return (List<Sucursal>)await _repository.GetAllAsync();
        }

        public async Task<Sucursal?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task UpdateAsync(Guid id, BranchDto dto)
        {
            var branch = await _repository.GetByIdAsync(id)
                         ?? throw new KeyNotFoundException("Sucursal no encontrada.");

            branch.Name = dto.Name.Trim();
            branch.Address = dto.Address.Trim();
            branch.Phone = dto.Phone.Trim();

            _repository.Update(branch);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var branch = await _repository.GetByIdAsync(id)
                         ?? throw new KeyNotFoundException("Sucursal no encontrada.");

            _repository.Delete(branch);
            await _repository.SaveChangesAsync();
        }
    }
}
