using System;
using Domain.DTOs.Categories;
using Domain.DTOs.Sale;
using Infrastructure.APIResult;

namespace Infrastructure.Interfaces;

public interface ISaleService
{
    Task<Result<IEnumerable<SaleGetDto>>> GetItemsAsync();
    Task<Result<SaleGetDto>> GetItemById(int id);
    Task<Result<SaleCreateResponseDto>> CreateItemAsync(SaleCreateDto dto);
    Task<Result<SaleUpdateResponseDto>> UpdateItemAsync(int id, SaleUpdateDto dto);
    Task<Result<string>> DeleteItemAsync(int id);
}
