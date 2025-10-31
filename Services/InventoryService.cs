using Api.DTOs;
using Api.Models;
using Api.Interface;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class InventoryService
    {
        private readonly IRepository<MateriaPrima> _rawMaterialRepo;
        private readonly IInventoryRepository _inventoryItemRepo;
        private readonly IRepository<MovimientoInventario> _inventoryMovementRepo;

        public InventoryService(
            IRepository<MateriaPrima> rawMaterialRepo,
            IInventoryRepository inventoryItemRepo,
            IRepository<MovimientoInventario> inventoryMovementRepo)
        {
            _rawMaterialRepo = rawMaterialRepo;
            _inventoryItemRepo = inventoryItemRepo;
            _inventoryMovementRepo = inventoryMovementRepo;
        }

        public async Task<MateriaPrima> CreateRawMaterialAsync(RawMaterialDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ValidationException("El nombre es requerido.");
            if (string.IsNullOrWhiteSpace(dto.Unit))
                throw new ValidationException("La unidad es requerida.");
            if (dto.MinStock < 0)
                throw new ValidationException("El stock mínimo debe ser >= 0.");

            var rawMaterial = new MateriaPrima
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Unit = dto.Unit,
                MinStock = dto.MinStock
            };

            await _rawMaterialRepo.AddAsync(rawMaterial);
            await _rawMaterialRepo.SaveChangesAsync();

            // Crear automáticamente el InventarioItem asociado con cantidad inicial 0
            var inventoryItem = new InventarioItem
            {
                Id = Guid.NewGuid(),
                RawMaterialId = rawMaterial.Id,
                CurrentQuantity = 0
            };
            await _inventoryItemRepo.AddAsync(inventoryItem);
            await _inventoryItemRepo.SaveChangesAsync();

            return rawMaterial;
        }

        public async Task<IEnumerable<InventarioItem>> GetInventoryAsync()
        {
            return await _inventoryItemRepo.Query()
                .Include(i => i.RawMaterial)
                .ToListAsync();
        }

        public async Task<InventarioItem?> GetInventoryItemAsync(Guid rawMaterialId)
        {
            return await _inventoryItemRepo.Query()
                .Include(i => i.RawMaterial)
                .FirstOrDefaultAsync(i => i.RawMaterialId == rawMaterialId);
        }

        public async Task<MovimientoInventario> RegisterMovementAsync(Guid rawMaterialId, InventoryMovementDto dto)
        {
            var inventoryItem = await GetInventoryItemAsync(rawMaterialId);

            if (inventoryItem == null)
            {
                // Crear inventario inicial con cantidad 0 si no existe
                inventoryItem = await CreateInitialInventoryItemAsync(rawMaterialId);
            }

            ValidateMovementQuantity(dto.Quantity);

            // Actualizar cantidad segn tipo de movimiento
            UpdateInventoryItemQuantity(inventoryItem, dto);

            _inventoryItemRepo.Update(inventoryItem);

            var movement = CreateInventoryMovement(rawMaterialId, dto);

            await _inventoryMovementRepo.AddAsync(movement);

            await _inventoryItemRepo.SaveChangesAsync();
            await _inventoryMovementRepo.SaveChangesAsync();

            return movement;
        }

        private async Task<InventarioItem> CreateInitialInventoryItemAsync(Guid rawMaterialId)
        {
            var inventoryItem = new InventarioItem
            {
                Id = Guid.NewGuid(),
                RawMaterialId = rawMaterialId,
                CurrentQuantity = 0
            };
            await _inventoryItemRepo.AddAsync(inventoryItem);
            await _inventoryItemRepo.SaveChangesAsync();
            return inventoryItem;
        }

        private void ValidateMovementQuantity(decimal quantity)
        {
            if (quantity <= 0)
                throw new ValidationException("La cantidad debe ser mayor a cero.");
        }

        private void UpdateInventoryItemQuantity(InventarioItem inventoryItem, InventoryMovementDto dto)
        {
            switch (dto.MovementType)
            {
                case "In":
                    inventoryItem.CurrentQuantity += dto.Quantity;
                    break;
                case "Out":
                case "Waste":
                    if (inventoryItem.CurrentQuantity < dto.Quantity)
                        throw new ValidationException("Cantidad insuficiente en inventario.");
                    inventoryItem.CurrentQuantity -= dto.Quantity;
                    break;
                default:
                    throw new ValidationException($"Tipo de movimiento invlido: {dto.MovementType}");
            }
        }

        private MovimientoInventario CreateInventoryMovement(Guid rawMaterialId, InventoryMovementDto dto)
        {
            return new MovimientoInventario
            {
                Id = Guid.NewGuid(),
                RawMaterialId = rawMaterialId,
                Quantity = dto.Quantity,
                MovementType = dto.MovementType,
                Date = dto.Date == default ? DateTime.UtcNow : dto.Date
            };
        }

    }
}
