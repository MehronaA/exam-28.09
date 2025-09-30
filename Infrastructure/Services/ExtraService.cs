using System;
using Domain.DTOs.Extra;
using Domain.DTOs.Products;
using Domain.DTOs.Sale;
using Domain.DTOs.StockAdjustment;
using Infrastructure.APIResult;
using Infrastructure.Data;
using Infrastructure.Enum;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class ExtraService(DataContext context) : IExtraService
{



    public async Task<Result<DashboardStatisticDto>> DashboardStatistic()
    {
        try
        {
            var totalProducts = await context.Products.CountAsync();

            var totalSales = await context.Sales.SumAsync(s => s.QuantitySold);

            var totalRevenue = await context.Sales
                .SumAsync(s => (decimal)s.QuantitySold * s.Product.Price);

            var dto = new DashboardStatisticDto
            {
                TotalProducts = totalProducts,
                TotalSales = totalSales,
                TotalRevenue = totalRevenue
            };

            return Result<DashboardStatisticDto>.Ok(dto);
        }
        catch (Exception)
        {
            return Result<DashboardStatisticDto>.Fail("Internal server error", ErrorType.Internal);
        }
    }





}
