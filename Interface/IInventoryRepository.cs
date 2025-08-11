using Api.Models;

namespace Api.Interface

{
    public interface IInventoryRepository : IRepository<InventarioItem>
    {
        Task<InventarioItem?> GetItemByRawMaterialAsync(Guid rawMaterialId);
        Task UpdateQuantityAsync(Guid rawMaterialId, decimal quantityChange, string movementType);
    }
}