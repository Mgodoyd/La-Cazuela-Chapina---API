using Api.DTOs;
using Api.Interface;
using Api.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IRepository<DetallePedido> _orderDetailRepo;
        private readonly IRepository<Usuario> _userRepo;
        private readonly IRepository<ProductoBase> _productRepo;

        public OrderService(
            IOrderRepository orderRepo,
            IRepository<DetallePedido> orderDetailRepo,
            IRepository<Usuario> userRepo,
            IRepository<ProductoBase> productRepo)
        {
            _orderRepo = orderRepo;
            _orderDetailRepo = orderDetailRepo;
            _userRepo = userRepo;
            _productRepo = productRepo;
        }

        public async Task<Pedido> CreateAsync(OrderDto dto)
        {

            var user = await _userRepo.GetByIdAsync(dto.UserId);
            if (user == null)
                throw new ValidationException("Usuario no encontrado.");

            var order = new Pedido
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                CreatedAt = dto.CreatedAt == default ? DateTime.UtcNow : dto.CreatedAt,
                Confirmed = dto.Confirmed,
                Stock = dto.Stock,
                Status = dto.Status,
            };

            await _orderRepo.AddAsync(order);

            foreach (var itemDto in dto.Items)
            {
                if (itemDto.Quantity <= 0)
                    throw new ValidationException("La cantidad debe ser mayor a cero.");

                var product = await _productRepo.GetByIdAsync(itemDto.ProductId)
                    ?? throw new ValidationException($"Producto no encontrado: {itemDto.ProductId}");

                var detail = new DetallePedido
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    Status = order.Status,
                    UnitPrice = itemDto.UnitPrice > 0 ? itemDto.UnitPrice : product.Price
                };
                await _orderDetailRepo.AddAsync(detail);
            }

            await _orderRepo.SaveChangesAsync();
            await _orderDetailRepo.SaveChangesAsync();

            var fullOrder = await _orderRepo.Query()
                .Include(o => o.User)
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == order.Id);

            return fullOrder!;
        }

        public async Task<IEnumerable<Pedido>> GetAllAsync()
        {
            return await _orderRepo.Query()
                .Include(o => o.Items)
                .ThenInclude(od => od.Product)
                .Include(o => o.User)
                .ToListAsync();
        }

        public async Task<Pedido?> GetByIdAsync(Guid id)
        {
            return await _orderRepo.Query()
                .Include(o => o.Items)
                .ThenInclude(od => od.Product)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task UpdateAsync(Guid id, OrderDto dto)
        {
            var order = await _orderRepo.Query()
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id)
                ?? throw new KeyNotFoundException("Pedido no encontrado.");

            order.Confirmed = dto.Confirmed;

            foreach (var detail in order.Items.ToList())  // .ToList() para evitar modificación durante iteración
            {
                _orderDetailRepo.Delete(detail);
            }

            foreach (var itemDto in dto.Items)
            {
                var detalleExistente = order.Items.FirstOrDefault(d => d.ProductId == itemDto.ProductId);
                if (detalleExistente != null)
                {
                    detalleExistente.Quantity = itemDto.Quantity;
                    detalleExistente.UnitPrice = itemDto.UnitPrice;
                    _orderDetailRepo.Update(detalleExistente);
                }
                else
                {
                    var nuevoDetalle = new DetallePedido
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        ProductId = itemDto.ProductId,
                        Quantity = itemDto.Quantity,
                        Status = order.Status,
                        UnitPrice = itemDto.UnitPrice
                    };
                    await _orderDetailRepo.AddAsync(nuevoDetalle);
                }
            }

            _orderRepo.Update(order);
            await _orderRepo.SaveChangesAsync();
            await _orderDetailRepo.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid orderId)
        {
            var order = await _orderRepo.Query()
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new KeyNotFoundException("Pedido no encontrado.");

            foreach (var detail in order.Items.ToList())
            {
                _orderDetailRepo.Delete(detail);
            }

            _orderRepo.Delete(order);

            await _orderDetailRepo.SaveChangesAsync();
            await _orderRepo.SaveChangesAsync();
        }
    }
}
