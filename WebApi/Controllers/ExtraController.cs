using System;
using Infrastructure.Enum;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/")]
public class ExtraController(IExtraService _service) : ControllerBase
{
    [HttpGet("product-statistics")]
    public async Task<IActionResult> GetProductsStatistics()
    {
        var result = await _service.ProductStatistic();

        if (!result.IsSuccess)
        {
            return StatusCode(500, result);
        }

        return Ok(result.Data);
    }
    [HttpGet("sales-by-date")]
    public async Task<IActionResult> GetByDate([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var result = await _service.ProductsWithDate(startDate, endDate);

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
    [HttpGet("stock-adjustment-history")]
    public async Task<IActionResult> GetHistory([FromQuery] int productId)
    {
        var result = await _service.StockAdjustmentHistory(productId);

        if (!result.IsSuccess)
        {
            return result.ErrorType switch
            {
                ErrorType.Validation => BadRequest(result),
                ErrorType.NotFound => NotFound(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        return Ok(result.Data);
    }
    [HttpGet("supplier-with-products")]
    public async Task<IActionResult> GetSuppliersWithProducts()
    {
        var result = await _service.SupplierWithProducts();

        if (!result.IsSuccess)
            return StatusCode(StatusCodes.Status500InternalServerError, result);

        return Ok(result.Data);
    }

    [HttpGet("top-sale-product")]
    public async Task<IActionResult> GetTop()
    {
        var result = await _service.TopSaleProduct();
        if (!result.IsSuccess) return StatusCode(500, result.Message);
        return Ok(result.Data);
    }

    [HttpGet("dashboard-statistics")]
    public async Task<IActionResult> GetStatistics()
    {
        var result = await _service.DashboardStatistic();

        if (!result.IsSuccess)
            return StatusCode(500, result);

        return Ok(result.Data);
    }

    [HttpGet("low-stock-products")]
    public async Task<IActionResult> GetLowStockProducts()
    {
        var result = await _service.LowStockProduct();

        if (!result.IsSuccess)
            return StatusCode(500, result);

        return Ok(result.Data);
    }

    [HttpGet("details/{id:int}")]
    public async Task<IActionResult> GetProductDetails(int id)
    {
        var result = await _service.ProductDetails(id);

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
    [HttpGet("category-with-products")]
    public async Task<IActionResult> GetCategoriesWithProductsAsync()
    {
        var result = await _service.CategoryWithProducts();
        if (!result.IsSuccess)
            return StatusCode(500, result);

        return Ok(result.Data);
    }
    [HttpGet("daily-revenue")]
    public async Task<IActionResult> GetDailyRevenue()
    {
        var result = await _service.DailyRevenue();
        if (!result.IsSuccess)
            return StatusCode(500, result);


        return Ok(result);
    }
}





    


  
    



