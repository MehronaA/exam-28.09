using System;
using Domain.DTOs.Categories;
using Domain.DTOs.Extra;
using Domain.DTOs.Products;
using Domain.Filters;
using Infrastructure.APIResult;

namespace Infrastructure.Interfaces;

public interface ICategoryService
{
    Task<PageResult<IEnumerable<CategoryGetDto>>> GetFilteredItemsAsync(CategoryFilter filter);
    Task<Result<CategoryGetDto>> GetItemByIdAsync(int id);
    Task<Result<CategoryCreateResponseDto>> CreateItemAsync(CategoryCreateDto dto);
    Task<Result<CategoryUpdateResponseDto>> UpdateItemAsync(int id, CategoryUpdateDto dto);
    Task<Result<string>> DeleteItemAsync(int id);
    Task<Result<IEnumerable<CategoryWithProducts>>> CategoryWithProducts();

}
