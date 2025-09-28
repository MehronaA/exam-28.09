using System;
using Domain.DTOs.Categories;
using Domain.DTOs.Products;
using Domain.Filters;
using Infrastructure.APIResult;

namespace Infrastructure.Interfaces;

public interface ICategoryService
{
    Task<Result<IEnumerable<CategoryGetDto>>> GetItemsAsync();
    Task<Result<CategoryGetDto>> GetItemById(int id);
    Task<Result<CategoryCreateResponseDto>> CreateItemAsync(CategoryCreateDto dto);
    Task<Result<CategoryUpdateResponseDto>> UpdateItemAsync(int id, CategoryUpdateDto dto);
    Task<Result<string>> DeleteItemAsync(int id);
}
