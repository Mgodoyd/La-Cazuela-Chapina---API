using Api.DTOs;
using Api.Interface;
using Api.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Api.Services
{
    public class SaleService
    {
        private readonly ISaleRepository _saleRepo;
        private readonly IRepository<DetalleVenta> _saleDetailRepo;
        private readonly IRepository<Usuario> _userRepo;
        private readonly IRepository<ProductoBase> _productRepo;

        public SaleService(
            ISaleRepository saleRepo,
            IRepository<DetalleVenta> saleDetailRepo,
            IRepository<Usuario> userRepo,
            IRepository<ProductoBase> productRepo
        )
        {
            _saleRepo = saleRepo;
            _saleDetailRepo = saleDetailRepo;
            _userRepo = userRepo;
            _productRepo = productRepo;
        }

        public async Task<Venta> RegisterAsync(SaleDto dto)
        {
            var user = await _userRepo.GetByIdAsync(dto.UserId)
                ?? throw new ValidationException("El usuario no existe.");

            var productIds = dto.Items.Select(i => i.ProductId).ToList();
            var products = await _productRepo.Query()
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            if (products.Count != dto.Items.Count)
                throw new ValidationException("Uno o más productos no existen.");

            foreach (var item in dto.Items)
            {
                var product = products.First(p => p.Id == item.ProductId);
                if (product.Stock < item.Quantity)
                    throw new ValidationException($"Stock insuficiente para el producto {product.Name}.");
            }

            var total = dto.Items.Sum(i => i.Quantity * i.UnitPrice);
            var sale = new Venta
            {
                Id = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                UserId = dto.UserId,
                Total = total
            };

            await _saleRepo.AddAsync(sale);

            foreach (var item in dto.Items)
            {
                var detail = new DetalleVenta
                {
                    Id = Guid.NewGuid(),
                    SaleId = sale.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                };
                await _saleDetailRepo.AddAsync(detail);

                var product = products.First(p => p.Id == item.ProductId);
                product.Stock -= item.Quantity;
                _productRepo.Update(product);
            }

            await _saleRepo.SaveChangesAsync();
            await _saleDetailRepo.SaveChangesAsync();
            await _productRepo.SaveChangesAsync();

            return sale;
        }

        public async Task<IEnumerable<Venta>> GetAllAsync()
        {
            return await _saleRepo.Query()
                .Include(s => s.Items)
                .ThenInclude(d => d.Product)
                .Include(s => s.User)
                .ToListAsync();
        }

        public async Task<Venta?> GetByIdAsync(Guid id)
        {
            return await _saleRepo.Query()
                .Include(s => s.Items)
                .ThenInclude(d => d.Product)
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}
