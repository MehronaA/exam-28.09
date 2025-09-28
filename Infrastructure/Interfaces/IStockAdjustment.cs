using System;
using Domain.DTOs.Sale;
using Domain.DTOs.StockAdjustment;
using Infrastructure.APIResult;

namespace Infrastructure.Interfaces;

public interface IStockAdjustment
{
    Task<Result<IEnumerable<StockAdjustmentGetDto>>> GetItemsAsync();
    Task<Result<StockAdjustmentGetDto>> GetItemById(int id);
    Task<Result<StockAdjustmentCreateResponseDto>> CreateItemAsync(StockAdjustmentCreateDto dto);
    Task<Result<StockAdjustmentUpdateResponseDto>> UpdateItemAsync(int id, StockAdjustmentUpdateDto dto);
    Task<Result<string>> DeleteItemAsync(int id);
}
