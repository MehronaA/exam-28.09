using Domain.DTOs.Categories;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorApp.Pages.Categories;

public class Edit(ICategoryService categoryService) : PageModel
{
    [BindProperty]
    public int CategoryId { get; set; }
    [BindProperty]
    public CategoryUpdateDto? CategoryUpdateDto { get; set; }
    public async Task OnGetAsync(int id)
    {
        var category = await categoryService.GetItemByIdAsync(id);
        
        CategoryId =category.Data!.Id;
        CategoryUpdateDto = new CategoryUpdateDto()
        {
            Name=category.Data.Name
        };
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var result = await categoryService.UpdateItemAsync(CategoryId, CategoryUpdateDto!);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message ?? "Не удалось сохранить изменения.");
            return Page();
        
        }
            
       return RedirectToPage("/Categories/Index");
    }
}