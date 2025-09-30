using System;
using Domain.DTOs.Supplier;
using Infrastructure.APIResult;

namespace Infrastructure.Interfaces;

public interface ISupplierService
{
    Task<Result<IEnumerable<SupplierGetDto>>> GetItemsAsync();
    Task<Result<SupplierGetDto>> GetItemByIdAsync(int id);
    Task<Result<SupplierCreateResponseDto>> CreateItemAsync(SupplierCreateDto dto);
    Task<Result<SupplierUpdateResponseDto>> UpdateItemAsync(int id, SupplierUpdateDto dto);
    Task<Result<string>> DeleteItemAsync(int id);
    Task<Result<IEnumerable<SupplierWithProductsDto>>> SupplierWithProducts();

}
