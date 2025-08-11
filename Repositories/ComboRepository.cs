using Api.Data;
using Api.Interface;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories

{
    public class ComboRepository : EfRepository<Combo>, IComboRepository
    {
        public ComboRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Combo?> GetByIdWithProductsAsync(Guid id) =>
            await _context.Combo.Include(c => c.Products)
                                 .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<IEnumerable<Combo>> GetEditableCombosAsync() =>
            await _context.Combo.Where(c => c.Editable)
                                 .Include(c => c.Products)
                                 .ToListAsync();
    }
}