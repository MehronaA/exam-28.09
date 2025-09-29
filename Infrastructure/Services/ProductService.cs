using System;
using System.Security.Cryptography;
using Domain.DTOs.Categories;
using Domain.DTOs.Products;
using Domain.Entities;
using Domain.Filters;
using Infrastructure.APIResult;
using Infrastructure.Data;
using Infrastructure.Enum;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class ProductService(DataContext context) : IProductService
{
    public async Task<PageResult<IEnumerable<ProductGetDto>>> GetFilteredItemsAsync(ProductFilter filter)
    {
       
        var query = context.Products.Where(p => p.QuantityInStock > 0).AsQueryable();

        var totalCount = query.Count();

        if (filter.Keyword != null)
        {
            query = query.Where(p => EF.Functions.Like($"%{p.Name.ToLower()}%", filter.Keyword.ToLower().Trim()));
        }

        if (filter.MinPrice != null)
        {
            query = query.Where(p => p.Price >= filter.MinPrice);
        }

        if (filter.MaxPrice != null)
        {
            query = query.Where(p => p.Price <= filter.MaxPrice);
        }

        var items = await query
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Skip((filter.Page - 1) * filter.Size)
            .Take(filter.Size)
            .Select(p => new ProductGetDto()
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                QuantityInStock = p.QuantityInStock,
                CategoryName = p.Category.Name,
                SupplierName = p.Supplier.Name
            }).ToListAsync();
        return PageResult<IEnumerable<ProductGetDto>>.Ok(items, filter.Page, filter.Size, totalCount);
    }

    public async Task<Result<ProductGetDto>> GetItemByIdAsync(int id)
    {
        var dto = await context.Products
        .AsNoTracking()
        .Include(p => p.Category)
        .Include(p=>p.Supplier)
        .Where(p => p.Id == id)
        .Select(p => new ProductGetDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            QuantityInStock = p.QuantityInStock,
            CategoryName = p.Category.Name,
            SupplierName = p.Supplier.Name   
        })
        .FirstOrDefaultAsync();

    if (dto is null)
        return Result<ProductGetDto>.Fail("Product not found", ErrorType.NotFound);

    return Result<ProductGetDto>.Ok(dto);
    }
    public async Task<Result<ProductCreateResponseDto>> CreateItemAsync(ProductCreateDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return Result<ProductCreateResponseDto>.Fail("Name is required", ErrorType.Validation);
            }

            if (dto.Name.Trim().Length > 100 || dto.Name.Trim().Length < 2)
            { 
                return Result<ProductCreateResponseDto>.Fail("Name shouldn't have less than 2 charackters and more than 100 charackters", ErrorType.Validation);
            }

            if (dto.Price < 0)
            { 
                return Result<ProductCreateResponseDto>.Fail("Price cannot be negative", ErrorType.Validation);
            }
            if (dto.QuantityInStock < 0)
            {
                return Result<ProductCreateResponseDto>.Fail("Quantity cannot be negative", ErrorType.Validation);
            }

            var categoryExists = await context.Categories.AnyAsync(c => c.Id == dto.CategoryId);

            if (!categoryExists)
            {
                return Result<ProductCreateResponseDto>.Fail("Category not found", ErrorType.NotFound);
            }

            var supplierExists = await context.Suppliers.AnyAsync(s => s.Id == dto.SupplierId);

            if (!supplierExists)
            {
                return Result<ProductCreateResponseDto>.Fail("Supplier not found", ErrorType.NotFound);
            }

            var exist = await context.Products.AnyAsync(p => p.Name == dto.Name);

            if (exist)
            {
                return Result<ProductCreateResponseDto>.Fail("Product with this name already exists", ErrorType.Conflict);
            }

            var newProduct = new Product
            {
                Name = dto.Name.Trim(),
                Price = dto.Price,
                CategoryId = dto.CategoryId,
                SupplierId = dto.SupplierId,
                QuantityInStock= dto.QuantityInStock
            };

            await context.Products.AddAsync(newProduct);
            await context.SaveChangesAsync();

            var result = await context.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Where(p => p.Id == newProduct.Id)
            .Select(p => new ProductCreateResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                QuantityInStock = p.QuantityInStock,
                CategoryName = p.Category.Name,
                SupplierName = p.Supplier.Name
            }).FirstAsync();

            return Result<ProductCreateResponseDto>.Ok(result);
        }
        catch (System.Exception)
        {
            return Result<ProductCreateResponseDto>.Fail("Internal server error", ErrorType.Internal);
        }
        
    }
    public async Task<Result<ProductUpdateResponceDto>> UpdateItemAsync(int id, ProductUpdateDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return Result<ProductUpdateResponceDto>.Fail("Name is required", ErrorType.Validation);
            }

            if (dto.Name.Trim().Length > 100 || dto.Name.Trim().Length < 2)
            {
                return Result<ProductUpdateResponceDto>.Fail("Name shouldn't have less than 2 charackters and more than 100 charackters", ErrorType.Validation);
            }

            if (dto.Price < 0)
            {
                return Result<ProductUpdateResponceDto>.Fail("PRice cannot be negative", ErrorType.Validation);
            }
            var categoryExists = await context.Categories.AnyAsync(c => c.Id == dto.CategoryId);

            if (!categoryExists)
            {
                return Result<ProductUpdateResponceDto>.Fail("Category not found", ErrorType.NotFound);
            }

            var supplierExists = await context.Suppliers.AnyAsync(s => s.Id == dto.SupplierId);

            if (!supplierExists)
            {
                return Result<ProductUpdateResponceDto>.Fail("Supplier not found", ErrorType.NotFound);
            }

            var exist = await context.Products.FindAsync(id);

            if (exist == null)
            {
                return Result<ProductUpdateResponceDto>.Fail("Product to update  does not exists", ErrorType.NotFound);
            }

            var noChange = exist.Name.ToLower() == dto.Name.Trim().ToLower() && exist.Price == dto.Price && exist.CategoryId == dto.CategoryId && exist.SupplierId == dto.SupplierId;

            if (noChange)
            {
                return Result<ProductUpdateResponceDto>.Fail("No changes were made", ErrorType.NoChange);
            }

            exist.Name = dto.Name.Trim();
            exist.Price = dto.Price;
            exist.CategoryId = dto.CategoryId;
            exist.SupplierId = dto.SupplierId;

            await context.SaveChangesAsync();

            var result = await context.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Where(p => p.Id == id)
            .Select(p => new ProductUpdateResponceDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                QuantityInStock = p.QuantityInStock,
                CategoryName = p.Category.Name,
                SupplierName = p.Supplier.Name
            }).FirstAsync();

            return Result<ProductUpdateResponceDto>.Ok(result);
        }
        catch (System.Exception)
        {
            return Result<ProductUpdateResponceDto>.Fail("Internal server error", ErrorType.Internal);
        }
    }

    public async Task<Result<string>> DeleteItemAsync(int id)
    {
        try
        {
            var exist = await context.Products.FindAsync(id);

            if (exist == null)
            {
                return Result<string>.Fail("Category to delete not found", ErrorType.NotFound);
            }

            context.Products.Remove(exist);
            await context.SaveChangesAsync();
            return Result<string>.Ok("Deleted succesfully");

        }
        catch (System.Exception)
        {
            return Result<string>.Fail("Internal server error", ErrorType.Internal);
        }

    }


}
