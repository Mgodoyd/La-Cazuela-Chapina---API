using Api.Data;
using Api.Models;
using Microsoft.EntityFrameworkCore;
using Api.Interface;

namespace Api.Repositories

{
     public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        public OrderRepository(ApplicationDbContext context) => _context = context;

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

        public async Task AddAsync(Pedido order)
        {
            _context.Order.Add(order);
            await _context.SaveChangesAsync();
        }
    }
}