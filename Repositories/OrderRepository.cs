using Api.Data;
using Api.Models;
using Microsoft.EntityFrameworkCore;
using Api.Interface;

namespace Api.Repositories

{
    public class OrderRepository : EfRepository<Pedido>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Pedido?> GetByIdWithItemsAsync(Guid id) =>
            await _context.Order.Include(o => o.Items)
                                 .FirstOrDefaultAsync(o => o.Id == id);

        public async Task<IEnumerable<Pedido>> GetByUserAsync(Guid userId) =>
            await _context.Order.Where(o => o.UserId == userId)
                                 .Include(o => o.Items)
                                 .ToListAsync();

        public async Task ConfirmOrderAsync(Guid id)
        {
            var order = await _context.Order.FindAsync(id);
            if (order != null)
            {
                order.Confirmed = true;
                await _context.SaveChangesAsync();
            }
        }

    }
}