using System;
using Domain.DTOs.Extra;
using Domain.DTOs.Products;
using Domain.DTOs.Sale;
using Domain.DTOs.StockAdjustment;
using Infrastructure.APIResult;
using Infrastructure.Data;
using Infrastructure.Enum;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class ExtraService(DataContext context) : IExtraService
{
    public async Task<Result<IEnumerable<CategoryWithProducts>>> CategoryWithProducts()
    {
        try
    {
        var categories = await context.Categories
            .AsNoTracking()
            .Include(c => c.Products) 
            .Select(c => new CategoryWithProducts
            {
                Id = c.Id,
                Name = c.Name,
                Products = c.Products.Select(p => new ProductGetDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price= p.Price
                }).ToList()
            })
            .ToListAsync();

        return Result<IEnumerable<CategoryWithProducts>>.Ok(categories);
    }
    catch (Exception)
    {
        return Result<IEnumerable<CategoryWithProducts>>.Fail("Internal server error", ErrorType.Internal);
    }
    }

    public async Task<Result<IEnumerable<ReportsDailyRevenueDto>>> DailyRevenue()
    {
        try
    {
        var today = DateTime.UtcNow.Date;      
        var start = today.AddDays(-7);              

        var grouped = await context.Sales
            .AsNoTracking()
            .Where(s => s.SaleDate >= start )
            .GroupBy(s => s.SaleDate.Date)
            .Select(g => new ReportsDailyRevenueDto
            {
                Date = g.Key,
                Revenue = g.Sum(s => (decimal)s.QuantitySold * s.Product.Price)
            })
            .ToListAsync();

        return Result<IEnumerable<ReportsDailyRevenueDto>>.Ok(grouped);
    }
    catch
    {
        return Result<IEnumerable<ReportsDailyRevenueDto>>.Fail("Internal server error", ErrorType.Internal);
    }
    }

    public async Task<Result<DashboardStatisticDto>> DashboardStatistic()
    {
         try
    {
        var totalProducts = await context.Products.CountAsync();

        var totalSales = await context.Sales.SumAsync(s => s.QuantitySold);

        var totalRevenue = await context.Sales
            .SumAsync(s => (decimal)s.QuantitySold * s.Product.Price);

        var dto = new DashboardStatisticDto
        {
            TotalProducts = totalProducts,
            TotalSales = totalSales,
            TotalRevenue = totalRevenue
        };

        return Result<DashboardStatisticDto>.Ok(dto);
    }
    catch (Exception)
    {
        return Result<DashboardStatisticDto>.Fail("Internal server error", ErrorType.Internal);
    }
    }

    public async Task<Result<IEnumerable<LowStockProductsDto>>> LowStockProduct()
    {
         try
    {
        var products = await context.Products
            .AsNoTracking()
            .Where(p => p.QuantityInStock < 5)
            .Select(p => new LowStockProductsDto
            {
                Id = p.Id,
                Name = p.Name,
                QuantityInStock = p.QuantityInStock
            })
            .ToListAsync();

        return Result<IEnumerable<LowStockProductsDto>>.Ok(products);
    }
    catch (Exception)
    {
        return Result<IEnumerable<LowStockProductsDto>>.Fail("Internal server error", ErrorType.Internal);
    }
    }

    public async Task<Result<GetProductsDetailsDto>> ProductDetails(int id)
    {
        try
    {
        var product = await context.Products
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new GetProductsDetailsDto
            {
                ProductId = p.Id,
                ProductName = p.Name,
                Price = p.Price,
                QuantityInStock = p.QuantityInStock,
                Supplier = p.Supplier.Name,
                Sales = p.Sales.Select(s => new SaleGetDto
                {
                    QuantitySold = s.QuantitySold,
                    SaleDate = s.SaleDate
                }).ToList(),
                StockAdjustments = p.StockAdjustments.Select(a => new StockAdjustmentGetDto
                {
                    AdjustmentAmount = a.AdjustmentAmount,
                    AdjustmentDate = a.AdjustmentDate,
                    Reason = a.Reason
                }).ToList()
            })
            .FirstOrDefaultAsync();

            if (product is null)
            {
                return Result<GetProductsDetailsDto>.Fail("Product not found", ErrorType.NotFound);
            }

        return Result<GetProductsDetailsDto>.Ok(product);
    }
    catch (Exception)
    {
        return Result<GetProductsDetailsDto>.Fail("Internal server error", ErrorType.Internal);
    }   
    }

    public async Task<Result<ProductsStatisticsDto>> ProductStatistic()
    {
        try
        {
            var totalProducts = await context.Products.CountAsync();

            decimal averagePrice = 0;
            if (totalProducts > 0)
            {
                averagePrice = await context.Products.AverageAsync(p => p.Price);
            }

            var totalSold = await context.Sales.SumAsync(s => s.QuantitySold);

            var dto = new ProductsStatisticsDto
            {
                TotalProducts = totalProducts,
                AveragePrice = averagePrice,
                TotalSold = totalSold
            };

            return Result<ProductsStatisticsDto>.Ok(dto);
        }
        catch (Exception)
        {
            return Result<ProductsStatisticsDto>.Fail("Internal server error", ErrorType.Internal);
        }
    
    }



    public async Task<Result<IEnumerable<ProductsWithTimeIntervalDto>>> ProductsWithDate(DateTime startDate, DateTime endDate)
    {
        try
    {
            if (endDate < startDate)
            {
                return Result<IEnumerable<ProductsWithTimeIntervalDto>>.Fail("endDate must be >= startDate", ErrorType.Validation);
            }

        var start = startDate.Date;
        var end = endDate.Date.AddDays(1);

        var items = await context.Sales
            .AsNoTracking()
            .Where(s => s.SaleDate >= start && s.SaleDate < end)
            .OrderBy(s => s.SaleDate)
            .Select(s => new ProductsWithTimeIntervalDto
            {
                Id = s.Id,                    
                ProductName = s.Product.Name,
                QuantitySold = s.QuantitySold,
                SaleDate = s.SaleDate
            })
            .ToListAsync();

        return Result<IEnumerable<ProductsWithTimeIntervalDto>>.Ok(items);
    }
    catch
    {
        return Result<IEnumerable<ProductsWithTimeIntervalDto>>.Fail("Internal server error", ErrorType.Internal);
    }
    }

    public async Task<Result<IEnumerable<StockAdjustmentHistoryDto>>> StockAdjustmentHistory(int id)
    {
        try
    {
            if (id <= 0)
            {
                return Result<IEnumerable<StockAdjustmentHistoryDto>>.Fail("productId is required", ErrorType.Validation);
            }

        var productExists = await context.Products.AnyAsync(p => p.Id == id);
            if (!productExists)
            {
                return Result<IEnumerable<StockAdjustmentHistoryDto>>.Fail("Product not found", ErrorType.NotFound);
            }

        var items = await context.StockAdjustments
            .AsNoTracking()
            .Where(a => a.ProductId == id)
            .OrderBy(a => a.AdjustmentDate) 
            .Select(a => new StockAdjustmentHistoryDto
            {
                AdjustmentDate = a.AdjustmentDate,
                Amount = a.AdjustmentAmount,
                Reason = a.Reason
            })
            .ToListAsync();

        return Result<IEnumerable<StockAdjustmentHistoryDto>>.Ok(items);
    }
    catch
    {
        return Result<IEnumerable<StockAdjustmentHistoryDto>>.Fail("Internal server error", ErrorType.Internal);
    }
    }

    public async Task<Result<IEnumerable<SupplierWithProductsDto>>> SupplierWithProducts()
    {
        try
    {
        var suppliers = await context.Suppliers
            .AsNoTracking()
            .Include(s => s.Products)
            .Select(s => new SupplierWithProductsDto
            {
                SupplierId = s.Id,
                SupplierName = s.Name,
                ProductName = s.Products.Select(p => p.Name).ToList()
            })
            .ToListAsync();

        return Result<IEnumerable<SupplierWithProductsDto>>.Ok(suppliers);
    }
    catch
    {
        return Result<IEnumerable<SupplierWithProductsDto>>.Fail("Internal server error", ErrorType.Internal);
    }
    }

public async Task<Result<IEnumerable<SalesTopProductsDto>>> TopSaleProduct()
{
    try
    {

        var items = await context.Sales
            .AsNoTracking()
            .GroupBy(s =>  s.Product.Name )
            .Select(g => new SalesTopProductsDto
            {
                ProductName = g.Key,
                TotalSold = g.Sum(x => x.QuantitySold)
            })
            .OrderByDescending(x => x.TotalSold)
            .ThenBy(x => x.ProductName)
            .Take(5)
            .ToListAsync();

        return Result<IEnumerable<SalesTopProductsDto>>.Ok(items);
    }
    catch
    {
        return Result<IEnumerable<SalesTopProductsDto>>.Fail("Internal server error", ErrorType.Internal);
    }
    }
}
