using Api.Models;

namespace Api.Interface

{
    public interface IComboRepository : IRepository<Combo>
    {
        Task<Combo?> GetByIdWithProductsAsync(Guid id);
        Task<IEnumerable<Combo>> GetEditableCombosAsync();
    }
}