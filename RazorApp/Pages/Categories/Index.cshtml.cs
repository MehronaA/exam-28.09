using Domain.DTOs.Categories;
using Infrastructure.APIResult;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorApp.Pages.Categories;

public class Index(ICategoryService categoryService) : PageModel
{
    public IEnumerable<CategoryGetDto>? Categories { get; set; } = [];
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        var result = await categoryService.GetItemsAsync();

        if (result.IsSuccess)
        {
            Categories = result.Data;
        }
        else
        {
            ErrorMessage = result.Message;
            Categories = [];
        }
    }
}