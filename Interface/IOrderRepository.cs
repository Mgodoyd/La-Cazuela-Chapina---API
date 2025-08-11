using Api.Models;

namespace Api.Interface

{
    public interface IOrderRepository : IRepository<Pedido>
    {
        Task<Pedido?> GetByIdWithItemsAsync(Guid id);
        Task<IEnumerable<Pedido>> GetByUserAsync(Guid userId);
        Task ConfirmOrderAsync(Guid id);
    }
}