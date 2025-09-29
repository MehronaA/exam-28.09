using System;
using Domain.DTOs.Extra;
using Infrastructure.APIResult;

namespace Infrastructure.Interfaces;

public interface IExtraService
{
    Task<Result<IEnumerable<CategoryWithProducts>>> CategoryWithProducts();
    Task<Result<DashboardStatisticDto>> DashboardStatistic();
    Task<Result<GetProductsDetailsDto>> ProductDetails(int id);
    Task<Result<IEnumerable<LowStockProductsDto>>> LowStockProduct();
    Task<Result<ProductsStatisticsDto>> ProductStatistic();
    Task<Result<IEnumerable<ProductsWithTimeIntervalDto>>> ProductsWithDate(DateTime startDate, DateTime endDate);
    Task<Result<IEnumerable<ReportsDailyRevenueDto>>> DailyRevenue();
    Task<Result<IEnumerable<SalesTopProductsDto>>> TopSaleProduct();
    Task<Result<IEnumerable<StockAdjustmentHistoryDto>>> StockAdjustmentHistory(int id);
    Task<Result<IEnumerable<SupplierWithProductsDto>>> SupplierWithProducts();


}
