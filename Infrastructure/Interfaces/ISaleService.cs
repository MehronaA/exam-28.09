using System;
using Domain.DTOs.Categories;
using Domain.DTOs.Sale;
using Domain.Filters;
using Infrastructure.APIResult;

namespace Infrastructure.Interfaces;

public interface ISaleService
{
    Task<PageResult<IEnumerable<SaleGetDto>>> GetFilteredItemsAsync(SaleFilter filter);
    Task<Result<SaleGetDto>> GetItemByIdAsync(int id);
    Task<Result<SaleCreateResponseDto>> CreateItemAsync(SaleCreateDto dto);
    Task<Result<SaleUpdateResponseDto>> UpdateItemAsync(int id, SaleUpdateDto dto);
    Task<Result<string>> DeleteItemAsync(int id);
    Task<Result<IEnumerable<ReportsDailyRevenueDto>>> DailyRevenue();
    Task<Result<IEnumerable<SalesTopProductsDto>>> TopSaleProduct();


}
