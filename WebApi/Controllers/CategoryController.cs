using System;
using System.Reflection.Emit;
using Domain.DTOs.Categories;
using Domain.Filters;
using Infrastructure.Enum;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/category")]
public class CategoryController(ICategoryService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetItemsAsync([FromQuery] CategoryFilter filter)
    {
        var result = await service.GetFilteredItemsAsync(filter);
        if (!result.IsSuccess)
        {
            return StatusCode(500, "Failed");
        }
        return Ok(result.Data);
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetItemByIdAsync(int id)
    {
        var result = await service.GetItemByIdAsync(id);
        if (!result.IsSuccess)
        {
            return result.ErrorType switch
            {
                ErrorType.NotFound => NotFound(result.Message),
                _ => StatusCode(500, "An unexpeted error occured.")
            };
        }
        return Ok(result.Data);
    }
    [HttpGet("category-with-products")]
    public async Task<IActionResult> GetCategoriesWithProductsAsync()
    {
        var result = await service.CategoryWithProducts();
        if (!result.IsSuccess)
            return StatusCode(500, result);

        return Ok(result.Data);
    }
    [HttpPost]
    public async Task<IActionResult> CreateItemAsync(CategoryCreateDto dto)
    {
        var result = await service.CreateItemAsync(dto);

        if (!result.IsSuccess)
        {
            return result.ErrorType switch
            {
                ErrorType.Validation => BadRequest(result),
                ErrorType.Conflict => Conflict(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };

        }

        return Ok(result.Data);
    
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateItemAsync(int id, [FromBody] CategoryUpdateDto dto)
    {
        var result = await service.UpdateItemAsync(id, dto);

        if (!result.IsSuccess)
        {
            return result.ErrorType switch
            {
                ErrorType.NoChange => Ok(result.Message),
                ErrorType.Conflict => Conflict(result.Message),
                ErrorType.NotFound => NotFound(result.Message),
                ErrorType.Validation => ValidationProblem(result.Message),
                _ => StatusCode(500, " An unexpeted error occured")

            };
        }
        return Ok(result.Data);
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteItemAsync(int id)
    { 
        var result = await service.DeleteItemAsync(id);

    if (!result.IsSuccess)
    {
        return result.ErrorType switch
        {
            ErrorType.NotFound => NotFound(result),
            _                  => StatusCode(500, result)
        };
    }

    return Ok(result); 
    }
}
