using Api.Data;
using Api.Interface;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories

{
    public class InventoryRepository : EfRepository<InventarioItem>, IInventoryRepository
    {
        public InventoryRepository(ApplicationDbContext context) : base(context) { }

        public async Task<InventarioItem?> GetItemByRawMaterialAsync(Guid rawMaterialId) =>
            await _context.Inventory
                          .FirstOrDefaultAsync(i => i.RawMaterialId == rawMaterialId);

        public async Task UpdateQuantityAsync(Guid rawMaterialId, decimal quantityChange, string movementType)
        {
            var item = await GetItemByRawMaterialAsync(rawMaterialId);

            if (item == null)
            {
                item = new InventarioItem
                {
                    RawMaterialId = rawMaterialId,
                    CurrentQuantity = 0
                };
                _context.Inventory.Add(item);
            }

            item.CurrentQuantity += quantityChange;

            _context.InventoryMovement.Add(new MovimientoInventario
            {
                RawMaterialId = rawMaterialId,
                Quantity = quantityChange,
                MovementType = movementType,
                Date = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
        }

    }

}