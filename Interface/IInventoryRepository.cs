using Api.Models;

namespace Api.Interface

{
    public interface IInventoryRepository
    {
        Task<InventarioItem?> GetItemByRawMaterialAsync(Guid rawMaterialId);
        Task UpdateQuantityAsync(Guid rawMaterialId, decimal quantityChange, string movementType);
        Task<IEnumerable<InventarioItem>> GetAllAsync();
    }
}