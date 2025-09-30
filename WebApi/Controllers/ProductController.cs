using System;
using Domain.DTOs.Products;
using Domain.Filters;
using Infrastructure.Enum;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;


[ApiController]
[Route("api/product")]

public class ProductController(IProductService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetFilteredItemsAsync([FromQuery] ProductFilter filter)
    {
        var result = await service.GetFilteredItemsAsync(filter);
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
    [HttpGet("product-statistics")]
    public async Task<IActionResult> GetProductsStatistics()
    {
        var result = await service.ProductStatistic();

        if (!result.IsSuccess)
        {
            return StatusCode(500, result);
        }

        return Ok(result.Data);
    }
    [HttpGet("sales-by-date")]
    public async Task<IActionResult> GetByDate([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var result = await service.ProductsWithDate(startDate, endDate);

        if (!result.IsSuccess)
        {
            return result.ErrorType switch
            {
                ErrorType.Validation => BadRequest(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }
        return Ok(result);
    }
    [HttpGet("low-stock-products")]
    public async Task<IActionResult> GetLowStockProducts()
    {
        var result = await service.LowStockProduct();

        if (!result.IsSuccess)
            return StatusCode(500, result);

        return Ok(result.Data);
    }
    [HttpGet("details/{id:int}")]
    public async Task<IActionResult> GetProductDetails(int id)
    {
        var result = await service.ProductDetails(id);

        if (!result.IsSuccess)
        {
            return result.ErrorType switch
            {
                ErrorType.NotFound => NotFound(result),
                _ => StatusCode(500, result)
            };
        }

        return Ok(result.Data);
    }
    [HttpPost]
    public async Task<IActionResult> CreateItemAsync(ProductCreateDto dto)
    {
        var result = await service.CreateItemAsync(dto);

        if (!result.IsSuccess)
        {
            return result.ErrorType switch
            {
                ErrorType.Validation => BadRequest(result),
                ErrorType.Conflict => Conflict(result),
                _ => StatusCode(500, result)
            };

        }
        return Ok(result.Data);
    
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateItemAsync(int id, [FromBody] ProductUpdateDto dto)
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
