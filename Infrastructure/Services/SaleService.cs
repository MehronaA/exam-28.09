using System;
using Domain.DTOs.Sale;
using Domain.Entities;
using Domain.Filters;
using Infrastructure.APIResult;
using Infrastructure.Data;
using Infrastructure.Enum;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class SaleService(DataContext context) : ISaleService
{
    public async Task<PageResult<IEnumerable<SaleGetDto>>> GetFilteredItemsAsync(SaleFilter filter)
    {
        var query = context.Sales.AsQueryable();

        var totalCount = query.Count();

        if (filter.Keyword != null)
        {
            query = query.Include(s => s.Product).Where(s => EF.Functions.Like($"%{s.Product.Name.ToLower()}%", filter.Keyword.ToLower().Trim()));
        }

        if (filter.StartDate != null)
        {
            query = query.Where(s => s.SaleDate > filter.StartDate);
        }

        if (filter.EndDate != null)
        {
            query = query.Where(s => s.SaleDate < filter.EndDate);
        }
        if (filter.Page < 1)
        {
            filter.Page = 1;
        }

        var items = await query
        .Include(s => s.Product)
        .Skip((filter.Page - 1) * filter.Size)
        .Take(filter.Size)
        .Select(s => new SaleGetDto()
        {
            Id = s.Id,
            ProductName = s.Product.Name,
            QuantitySold = s.QuantitySold,
            SaleDate = s.SaleDate
        }).ToListAsync();
        return PageResult<IEnumerable<SaleGetDto>>.Ok(items, filter.Page, filter.Size, totalCount);
    }
    public async Task<Result<SaleGetDto>> GetItemByIdAsync(int id)
    {
        try
        {
            var exist = await context.Sales
            .Include(s => s.Product)
            .FirstOrDefaultAsync(s => s.Id == id);

            if (exist == null)
            {
                return Result<SaleGetDto>.Fail("Sale not found", ErrorType.NotFound);
            }
            return Result<SaleGetDto>.Ok(new SaleGetDto
            {
                Id = id,
                ProductName = exist.Product.Name,
                QuantitySold = exist.QuantitySold,
                SaleDate = exist.SaleDate
            });
        }
        catch (System.Exception)
        {
            return Result<SaleGetDto>.Fail("Internal server error", ErrorType.Internal);
        }
    }
    public async Task<Result<SaleCreateResponseDto>> CreateItemAsync(SaleCreateDto dto)
    {
        try
        {
            if (dto.QuantitySold <= 0)
            {
                return Result<SaleCreateResponseDto>.Fail("QuantitySold cannot be negative", ErrorType.Validation);
            }

            var product = await context.Products.FirstOrDefaultAsync(p => p.Id == dto.ProductId);

            if (product is null)
            {
                return Result<SaleCreateResponseDto>.Fail("Product not found", ErrorType.NotFound);
            }

            if (product.QuantityInStock < dto.QuantitySold)
            {
                return Result<SaleCreateResponseDto>.Fail("Not enough", ErrorType.Conflict);
            }
            var newSale = new Sale
            {
                ProductId = dto.ProductId,
                QuantitySold = dto.QuantitySold
            };
            await context.Sales.AddAsync(newSale);

            product.QuantityInStock -= dto.QuantitySold;
            await context.SaveChangesAsync();

            var result = await context.Sales
                .Include(s => s.Product)
                .Where(s => s.Id == newSale.Id)
                .Select(s => new SaleCreateResponseDto
                {
                    Id = s.Id,
                    ProductName = s.Product.Name,
                    QuantitySold = s.QuantitySold,
                    SaleDate = s.SaleDate
                })
                .FirstAsync();
            return Result<SaleCreateResponseDto>.Ok(result);
        }
        catch (System.Exception)
        {
            return Result<SaleCreateResponseDto>.Fail("Internal server error", ErrorType.Internal);
        }

    }
    public async Task<Result<SaleUpdateResponseDto>> UpdateItemAsync(int id, SaleUpdateDto dto)
    {
        try
        {
            if (dto.QuantitySold <= 0)
            {
                return Result<SaleUpdateResponseDto>.Fail("QuantitySold cannot be negative", ErrorType.Validation);
            }

            var product = await context.Products.FirstOrDefaultAsync(p => p.Id == dto.ProductId);

            if (product is null)
            {
                return Result<SaleUpdateResponseDto>.Fail("Product not found", ErrorType.NotFound);
            }

            if (product.QuantityInStock < dto.QuantitySold)
            {
                return Result<SaleUpdateResponseDto>.Fail("Insufficient stock", ErrorType.Conflict);
            }
            var exist = await context.Sales.FindAsync(id);

            if (exist == null)
            {
                return Result<SaleUpdateResponseDto>.Fail("Sale to update  does not exists", ErrorType.NotFound);
            }

            var noChange = exist.ProductId == dto.ProductId && exist.QuantitySold == dto.QuantitySold;

            if (noChange)
            {
                return Result<SaleUpdateResponseDto>.Fail("No changes were made", ErrorType.NoChange);
            }

            exist.ProductId = dto.ProductId;
            exist.QuantitySold = dto.QuantitySold;
            await context.SaveChangesAsync();
            var result = await context.Sales
            .Include(s => s.Product)
            .Where(s => s.Id == id)
            .Select(s => new SaleUpdateResponseDto
            {
                Id = id,
                ProductId = s.ProductId,
                QuantitySold = s.QuantitySold,
                SaleDate = s.SaleDate
            }).FirstAsync();

            return Result<SaleUpdateResponseDto>.Ok(result);
        }
        catch (System.Exception)
        {
            return Result<SaleUpdateResponseDto>.Fail("Internal server error", ErrorType.Internal);
        }


    }
    public async Task<Result<string>> DeleteItemAsync(int id)
    {
        try
        {
            var exist = await context.Sales.FindAsync(id);

            if (exist == null)
            {
                return Result<string>.Fail("Sale to delete not found", ErrorType.NotFound);
            }

            context.Sales.Remove(exist);
            await context.SaveChangesAsync();
            return Result<string>.Ok("Deleted succesfully");
        }
        catch (System.Exception)
        {
            return Result<string>.Fail("Internal server error", ErrorType.Internal);

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
                .Where(s => s.SaleDate >= start)
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
    
    public async Task<Result<IEnumerable<SalesTopProductsDto>>> TopSaleProduct()
    {
        try
        {

            var items = await context.Sales
                .AsNoTracking()
                .GroupBy(s => s.Product.Name)
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
