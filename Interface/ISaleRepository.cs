using Api.Models;

namespace Api.Interface

{
    public interface ISaleRepository : IRepository<Venta>
    {
        Task<Venta?> GetByIdWithItemsAsync(Guid id);
        Task<IEnumerable<Venta>> GetByUserAsync(Guid userId);
        Task<IEnumerable<Venta>> GetByDateRangeAsync(DateTime start, DateTime end);

    }
}