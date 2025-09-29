using System;
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
        .Include(sa => sa.Product)
        .Select(sa => new StockAdjustmentGetDto
        {
            Id = sa.Id,
            ProductId = sa.ProductId,
            AdjustmentAmount = sa.AdjustmentAmount,
            Reason = sa.Reason
        })
        .ToListAsync();

        return Result<IEnumerable<StockAdjustmentGetDto>>.Ok(items);
    
    }
    public async Task<Result<StockAdjustmentGetDto>> GetItemByIdAsync(int id)
    {
        try
        {
            var exist = await context.StockAdjustments.FindAsync(id);

        if (exist == null)
        {
            return Result<StockAdjustmentGetDto>.Fail("Product with this name already exists", ErrorType.Conflict);
        }

        return Result<StockAdjustmentGetDto>.Ok(new StockAdjustmentGetDto
        {
            Id = id,
            ProductId = exist.ProductId,
            AdjustmentAmount = exist.AdjustmentAmount,
            Reason = exist.Reason
        });
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
            if (string.IsNullOrWhiteSpace(dto.Reason))
            {
                return Result<StockAdjustmentCreateResponseDto>.Fail("Reason is required", ErrorType.Validation);
            }

            var productExist = await context.Products.FindAsync(dto.ProductId);

            if (productExist == null)
            {
                return Result<StockAdjustmentCreateResponseDto>.Fail("Product not found", ErrorType.NotFound);
            }

            productExist.QuantityInStock += dto.AdjustmentAmount;
            

            var adjustment = new StockAdjustment
            {
                ProductId = dto.ProductId,
                AdjustmentAmount = dto.AdjustmentAmount,
                Reason = dto.Reason,
            };

            await context.StockAdjustments.AddAsync(adjustment);
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
        catch (System.Exception)
        {
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
    
}
