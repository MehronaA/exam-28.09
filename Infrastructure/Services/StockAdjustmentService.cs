using System;
using Domain.DTOs.Products;
using Domain.DTOs.StockAdjustment;
using Domain.Entities;
using Infrastructure.APIResult;
using Infrastructure.Data;
using Infrastructure.Enum;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Infrastructure.Services;

public class StockAdjustmentService(DataContext context) : IStockAdjustmentService
{
    public async Task<Result<IEnumerable<StockAdjustmentGetDto>>> GetItemsAsync()
    {
        var items = await context.StockAdjustments
        .Select(sa => new StockAdjustmentGetDto
        {
            Id = sa.Id,
            ProductName = sa.Product.Name,
            AdjustmentAmount = sa.AdjustmentAmount,
            Reason = sa.Reason,
            AdjustmentDate = sa.AdjustmentDate,
        })
        .ToListAsync();

        return Result<IEnumerable<StockAdjustmentGetDto>>.Ok(items);

    }
    public async Task<Result<StockAdjustmentGetDto>> GetItemByIdAsync(int id)
    {
        try
        {
            var exist = await context.StockAdjustments.Include(sa => sa.Product).FirstOrDefaultAsync(sa => sa.Id == id);

            if (exist == null)
            {
                return Result<StockAdjustmentGetDto>.Fail("Product with this name already exists", ErrorType.Conflict);
            }
            var result = new StockAdjustmentGetDto()
            {
                Id = exist.Id,
                ProductName = exist.Product.Name,
                AdjustmentAmount = exist.AdjustmentAmount,
                Reason = exist.Reason,
                AdjustmentDate = exist.AdjustmentDate
            };

            return Result<StockAdjustmentGetDto>.Ok(result);
        }
        catch (System.Exception)
        {
            return Result<StockAdjustmentGetDto>.Fail("Internal server error", ErrorType.Internal);
        }
    }

    public async Task<Result<StockAdjustmentCreateResponseDto>> CreateItemAsync(StockAdjustmentCreateDto dto)
    {
        try
        {
            if (dto.ProductId <= 0)
            {
                return Result<StockAdjustmentCreateResponseDto>.Fail("ProductId is required", ErrorType.Validation);
            }

            if (dto.AdjustmentAmount == 0)
            {
                return Result<StockAdjustmentCreateResponseDto>.Fail("AdjustmentAmount must be non-zero", ErrorType.Validation);
            }

            if (string.IsNullOrWhiteSpace(dto.Reason))
            {
                return Result<StockAdjustmentCreateResponseDto>.Fail("Reason is required", ErrorType.Validation);
            }

            // if (reason.Length > 300)
            // {
            //     return Result<StockAdjustmentCreateResponseDto>.Fail("Reason is too long (max 300)", ErrorType.Validation);
            // }

            var product = await context.Products.FirstOrDefaultAsync(p => p.Id == dto.ProductId);
            if (product is null)
            {
                return Result<StockAdjustmentCreateResponseDto>.Fail("Product not found", ErrorType.NotFound);
            }

            var newQty = product.QuantityInStock + dto.AdjustmentAmount;
            if (newQty < 0)
            {
                return Result<StockAdjustmentCreateResponseDto>.Fail("Resulting stock cannot be negative", ErrorType.Conflict);
            }

            var adjustment = new StockAdjustment()
            {
                ProductId = dto.ProductId,
                AdjustmentAmount = dto.AdjustmentAmount,
                Reason = dto.Reason
            };

            await context.StockAdjustments.AddAsync(adjustment);
            product.QuantityInStock = newQty;

            await context.SaveChangesAsync();

            var result = new StockAdjustmentCreateResponseDto
            {
                Id = adjustment.Id,
                ProductId = adjustment.ProductId,
                AdjustmentAmount = adjustment.AdjustmentAmount,
                Reason = adjustment.Reason,
                AdjustmentDate = adjustment.AdjustmentDate
            };

            return Result<StockAdjustmentCreateResponseDto>.Ok(result, "Stock adjusted successfully");

        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message);
            return Result<StockAdjustmentCreateResponseDto>.Fail("Internal server error", ErrorType.Internal);
        }
    }
    public async Task<Result<StockAdjustmentUpdateResponseDto>> UpdateItemAsync(int id, StockAdjustmentUpdateDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Reason))
            {
                return Result<StockAdjustmentUpdateResponseDto>.Fail("Reason is required", ErrorType.Validation);
            }

            var productExist = await context.Products.FindAsync(dto.ProductId);

            if (productExist == null)
            {
                return Result<StockAdjustmentUpdateResponseDto>.Fail("Product not found", ErrorType.NotFound);
            }
            productExist.QuantityInStock += dto.AdjustmentAmount;

            var exist = await context.StockAdjustments.FindAsync(id);
            if (exist == null)
            {
                return Result<StockAdjustmentUpdateResponseDto>.Fail("Stock Adjustment to update  does not exists", ErrorType.NotFound);
            }
            var noChange = exist.ProductId == dto.ProductId && exist.AdjustmentAmount == dto.AdjustmentAmount && exist.Reason.ToLower() == dto.Reason.Trim().ToLower();
            if (noChange)
            {
                return Result<StockAdjustmentUpdateResponseDto>.Fail("No changes were made", ErrorType.NoChange);
            }

            exist.ProductId = dto.ProductId;
            exist.AdjustmentAmount = dto.AdjustmentAmount;
            exist.Reason = dto.Reason;
            await context.SaveChangesAsync();
            var result = new StockAdjustmentUpdateResponseDto
            {
                Id = exist.Id,
                ProductId = exist.ProductId,
                AdjustmentAmount = exist.AdjustmentAmount,
                Reason = exist.Reason
            };
            return Result<StockAdjustmentUpdateResponseDto>.Ok(result);
        }
        catch (System.Exception)
        {
            return Result<StockAdjustmentUpdateResponseDto>.Fail("Internal server error", ErrorType.Internal);
        }
    }
    public async Task<Result<string>> DeleteItemAsync(int id)
    {
        try
        {
            var exist = await context.StockAdjustments.FindAsync(id);

            if (exist == null)
            {
                return Result<string>.Fail("Stock Adjustment to delete not found", ErrorType.NotFound);
            }

            context.StockAdjustments.Remove(exist);
            await context.SaveChangesAsync();
            return Result<string>.Ok("Deleted succesfully");
        }
        catch (System.Exception)
        {
            return Result<string>.Fail("Internal server error", ErrorType.Internal);

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


    
}
