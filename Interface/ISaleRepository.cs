using Api.Models;

namespace Api.Interface

{
    public interface ISaleRepository
    {
        Task<Venta?> GetByIdWithItemsAsync(Guid id);
        Task<IEnumerable<Venta>> GetByUserAsync(Guid userId);
        Task<IEnumerable<Venta>> GetByDateRangeAsync(DateTime start, DateTime end);
        Task AddAsync(Venta sale);
    }
}