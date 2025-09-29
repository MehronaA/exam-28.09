using System;
using Domain.DTOs.Supplier;
using Domain.Entities;
using Infrastructure.APIResult;
using Infrastructure.Data;
using Infrastructure.Enum;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Infrastructure.Services;

public class SupplierService(DataContext context) : ISupplierService
{
    public async Task<Result<IEnumerable<SupplierGetDto>>> GetItemsAsync()
    {
        try
        {
            var items = await context.Suppliers.Select(s => new SupplierGetDto
            {
                Id = s.Id,
                Name = s.Name,
                Phone = s.Phone
            }).ToListAsync();
            return Result<IEnumerable<SupplierGetDto>>.Ok(items);
        }
        catch (System.Exception)
        {
            return Result<IEnumerable<SupplierGetDto>>.Fail("Internal server error", ErrorType.Internal);
        }
    }
    public async Task<Result<SupplierGetDto>> GetItemByIdAsync(int id)
    {
        try
        {
            var exist = await context.Suppliers.FindAsync(id);
            if (exist == null)
            {
                return Result<SupplierGetDto>.Fail("Supplier not found", ErrorType.NotFound);
            }

            return Result<SupplierGetDto>.Ok(new SupplierGetDto { Id = id, Name = exist.Name, Phone = exist.Phone }); 
        }
        catch (System.Exception)
        {
            return Result<SupplierGetDto>.Fail("Internal server error", ErrorType.Internal);
        }
        
    }

    public async Task<Result<SupplierCreateResponseDto>> CreateItemAsync(SupplierCreateDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return Result<SupplierCreateResponseDto>.Fail("Name is required", ErrorType.Validation);
            }
            if (dto.Name.Trim().Length > 150 || dto.Name.Trim().Length < 2)
            {
                return Result<SupplierCreateResponseDto>.Fail("Name shouldn't have less than 2 charackters and more than 150 charackters", ErrorType.Validation);
            }
            if (dto.Phone.Trim().Length > 11 || dto.Phone.Trim().Length < 7)
            {
                return Result<SupplierCreateResponseDto>.Fail("Phone shouldn't have less than 7 charackters and more than 11 charackters", ErrorType.Validation);
            }
            var exist = await context.Suppliers.FirstOrDefaultAsync(s => s.Name.ToLower() == dto.Name.Trim().ToLower());
            if (exist != null)
            {
                return Result<SupplierCreateResponseDto>.Fail("Supplier already exist", ErrorType.Conflict);
            }

            var newSupplier = new Supplier
            {
                Name = dto.Name,
                Phone = dto.Name
            };

            await context.Suppliers.AddAsync(newSupplier);
            await context.SaveChangesAsync();

            return Result<SupplierCreateResponseDto>.Ok(new SupplierCreateResponseDto
            {
                Id = newSupplier.Id,
                Name = newSupplier.Name,
                Phone = newSupplier.Phone
            });
        
        }
        catch (System.Exception)
        {
            return Result<SupplierCreateResponseDto>.Fail("Internal server error", ErrorType.Internal);
        }
    }

    public async Task<Result<SupplierUpdateResponseDto>> UpdateItemAsync(int id, SupplierUpdateDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return Result<SupplierUpdateResponseDto>.Fail("Name is required", ErrorType.Validation);
        }
        if (dto.Name.Trim().Length > 150 || dto.Name.Trim().Length < 2)
        {
            return Result<SupplierUpdateResponseDto>.Fail("Name shouldn't have less than 2 charackters and more than 150 charackters", ErrorType.Validation);
        }
        if (dto.Phone.Trim().Length > 11 || dto.Phone.Trim().Length < 7)
        {
            return Result<SupplierUpdateResponseDto>.Fail("Phone shouldn't have less than 7 charackters and more than 11 charackters", ErrorType.Validation);
        }
        var exist = await context.Suppliers.FirstOrDefaultAsync(s => s.Name.ToLower() == dto.Name.Trim().ToLower());
        if (exist == null)
        {
            return Result<SupplierUpdateResponseDto>.Fail("Supplier doesn't exist", ErrorType.NotFound);
        }
        var noChange = exist.Name.ToLower() == dto.Name.Trim().ToLower() && exist.Phone == dto.Phone;
        if (noChange)
        {
            return Result<SupplierUpdateResponseDto>.Fail("No changes were made", ErrorType.NoChange);
        }

        exist.Name = dto.Name.Trim();
        exist.Phone = dto.Phone;
        await context.SaveChangesAsync();

        var result = new SupplierUpdateResponseDto
        {
            Id = id,
            Name = exist.Name,
            Phone = exist.Phone
        };
        return Result<SupplierUpdateResponseDto>.Ok(result);
            
        }
        catch (System.Exception)
        {
            return Result<SupplierUpdateResponseDto>.Fail("Internal server error", ErrorType.Internal);
            
        }
        
    }
    public async Task<Result<string>> DeleteItemAsync(int id)
    {
        try
        {
            var exist = await context.Suppliers.FindAsync(id);

            if (exist == null)
            {
                return Result<string>.Fail("Category to delete not found", ErrorType.NotFound);
            }

            context.Suppliers.Remove(exist);
            await context.SaveChangesAsync();
            return Result<string>.Ok("Deleted succesfully");

        }
        catch (System.Exception)
        {
            return Result<string>.Fail("Internal server error", ErrorType.Internal);
        }
    }


}
