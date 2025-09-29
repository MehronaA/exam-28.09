using System;
using Domain.DTOs.Categories;
using Infrastructure.APIResult;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Enum;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using System.Linq.Expressions;

namespace Infrastructure.Services;

public class CategoryService(DataContext context) : ICategoryService
{
    public async Task<Result<IEnumerable<CategoryGetDto>>> GetItemsAsync()
    {
        try
        {
            var items = await context.Categories.Select(c => new CategoryGetDto
            {
                Id = c.Id,
                Name = c.Name
            }).ToListAsync();
            return Result<IEnumerable<CategoryGetDto>>.Ok(items);
        }
        catch (System.Exception)
        {
            return Result<IEnumerable<CategoryGetDto>>.Fail("Internal server error", ErrorType.Internal);
        }
        
       

    }
    public async Task<Result<CategoryGetDto>> GetItemByIdAsync(int id)
    {
        try
        {
            var exist = await context.Categories.FindAsync(id);

            if (exist == null)
            {
                return Result<CategoryGetDto>.Fail("Category not found", ErrorType.NotFound);
            }

            var foundById = new CategoryGetDto
            {
                Id = exist.Id,
                Name = exist.Name
            };

            return Result<CategoryGetDto>.Ok(foundById);
        }
        catch (System.Exception)
        {
            return Result<CategoryGetDto>.Fail("Internal server error", ErrorType.Internal);
        }
        
        

    }
    public async Task<Result<CategoryCreateResponseDto>> CreateItemAsync(CategoryCreateDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return Result<CategoryCreateResponseDto>.Fail("Name is required", ErrorType.Validation);
            }

            var uniqueName = await context.Categories.AnyAsync(c => c.Name.ToLower() == dto.Name.Trim().ToLower());

            if (uniqueName)
            {
                return Result<CategoryCreateResponseDto>.Fail("Category already exist", ErrorType.Conflict);
            }

            var newCategory = new Category
            {
                Name = dto.Name.Trim()
            };
            await context.Categories.AddAsync(newCategory);
            await context.SaveChangesAsync();

            return Result<CategoryCreateResponseDto>.Ok(new CategoryCreateResponseDto { Id = newCategory.Id, Name = newCategory.Name });
        }
        catch (System.Exception)
        {
            return Result<CategoryCreateResponseDto>.Fail("Internal server error", ErrorType.Internal);
        }

    }
    public async Task<Result<CategoryUpdateResponseDto>> UpdateItemAsync(int id, CategoryUpdateDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return Result<CategoryUpdateResponseDto>.Fail("Name is required.", ErrorType.Validation);
            }

            var exist = await context.Categories.FindAsync(id);

            if (exist == null)
            {
                return Result<CategoryUpdateResponseDto>.Fail("Category not found", ErrorType.NotFound);
            }

            var noChange = exist.Name.ToLower() == dto.Name.Trim().ToLower();

            if (noChange)
            {
                return Result<CategoryUpdateResponseDto>.Fail("No changes were made", ErrorType.NoChange);
            }

            exist.Name = dto.Name.Trim();
            await context.SaveChangesAsync();
            return Result<CategoryUpdateResponseDto>.Ok(new CategoryUpdateResponseDto { Id = id, Name = exist.Name });
            
        }
        catch (System.Exception)
        {
            return Result<CategoryUpdateResponseDto>.Fail("Internal server error", ErrorType.Internal);
        }
        
    }
    public async Task<Result<string>> DeleteItemAsync(int id)
    {
        try
        {
            var exist = await context.Categories.FindAsync(id);

            if (exist == null)
            {
                return Result<string>.Fail("Category to delete not found", ErrorType.NotFound);
            }

            context.Categories.Remove(exist);
            await context.SaveChangesAsync();
            return Result<string>.Ok("Deleted succesfully");

        }
        catch (System.Exception)
        {
            return Result<string>.Fail("Internal server error", ErrorType.Internal);
        }
        
    }
}
