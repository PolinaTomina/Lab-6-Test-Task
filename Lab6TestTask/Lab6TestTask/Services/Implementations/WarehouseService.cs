using Lab6TestTask.Data;
using Lab6TestTask.Models;
using Lab6TestTask.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lab6TestTask.Services.Implementations;

/// <summary>
/// WarehouseService.
/// Implement methods here.
/// </summary>
public class WarehouseService : IWarehouseService
{
    private readonly ApplicationDbContext _dbContext;

    public WarehouseService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Warehouse> GetWarehouseAsync()
    {
        var warehouse = await _dbContext.Warehouses
            .Select(w => new
            {
                Warehouse = w,
                TotalValue = w.Products
                .Where(p => p.Status == Enums.ProductStatus.ReadyForDistribution)
                .Sum(p => p.Price * p.Quantity)
            })
            .OrderByDescending(x => x.TotalValue)
            .Select(x => x.Warehouse)
            .FirstOrDefaultAsync();

        if (warehouse == null)
            throw new InvalidOperationException("Warehouse not found");

        return warehouse;
    }

    public async Task<IEnumerable<Warehouse>> GetWarehousesAsync()
    {
        var warehouses = await _dbContext.Warehouses
            .Where(w => w.Products.Any(p =>
            p.ReceivedDate.Year == 2025 && 
            p.ReceivedDate.Month >= 4 && 
            p.ReceivedDate.Month <= 6))
            .ToListAsync();

        return warehouses;
    }
}
