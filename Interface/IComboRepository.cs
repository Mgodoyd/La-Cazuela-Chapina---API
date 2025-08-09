using Api.Models;

namespace Api.Interface

{
     public interface IComboRepository
    {
        Task<Combo?> GetByIdWithProductsAsync(Guid id);
        Task<IEnumerable<Combo>> GetEditableCombosAsync();
        Task AddAsync(Combo combo);
    }
}