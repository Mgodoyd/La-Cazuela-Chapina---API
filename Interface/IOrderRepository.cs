using Api.Models;

namespace Api.Interface

{
    public interface IOrderRepository
    {
        Task<Pedido?> GetByIdWithItemsAsync(Guid id);
        Task<IEnumerable<Pedido>> GetByUserAsync(Guid userId);
        Task ConfirmOrderAsync(Guid id);
        Task AddAsync(Pedido order);
    }
}