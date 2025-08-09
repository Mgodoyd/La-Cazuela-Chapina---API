using Api.Models;
using Api.Data;
using Microsoft.EntityFrameworkCore;
using Api.Interface;
namespace Api.Repositories

{
    public class SaleRepository(ApplicationDbContext context) : ISaleRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Venta?> GetByIdWithItemsAsync(Guid id) =>
            await _context.Sales.Include(s => s.Items)
                                .FirstOrDefaultAsync(s => s.Id == id);

        public async Task<IEnumerable<Venta>> GetByUserAsync(Guid userId) =>
            await _context.Sales.Where(s => s.UserId == userId)
                                .Include(s => s.Items)
                                .ToListAsync();

        public async Task<IEnumerable<Venta>> GetByDateRangeAsync(DateTime start, DateTime end) =>
            await _context.Sales.Where(s => s.Date >= start && s.Date <= end)
                                .Include(s => s.Items)
                                .ToListAsync();

        public async Task AddAsync(Venta sale)
        {
            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();
        }
    }
}