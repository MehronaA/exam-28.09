using System;
using Domain.DTOs.Products;
using Domain.Filters;
using Infrastructure.APIResult;

namespace Infrastructure.Interfaces;

public interface IProductService
{
    Task<PageResult<IEnumerable<ProductGetDto>>> GetFilteredItemsAsync(ProductFilter filter);
    Task<Result<ProductGetDto>> GetItemByIdAsync(int id);
    Task<Result<ProductCreateResponseDto>> CreateItemAsync(ProductCreateDto dto);
    Task<Result<ProductUpdateResponceDto>> UpdateItemAsync(int id, ProductUpdateDto dto);
    Task<Result<string>> DeleteItemAsync(int id);
    Task<Result<GetProductsDetailsDto>> ProductDetails(int id);
    Task<Result<IEnumerable<LowStockProductsDto>>> LowStockProduct();
    Task<Result<ProductsStatisticsDto>> ProductStatistic();
    Task<Result<IEnumerable<ProductsWithTimeIntervalDto>>> ProductsWithDate(DateTime startDate, DateTime endDate);



    

}
