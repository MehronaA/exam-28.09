using System.Net.WebSockets;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorApp.Pages.Categories
{
    public class Delete(ICategoryService categoryService) : PageModel
    {
    public int Id { get; set; }
    
    public void OnGetAsync(int id)
    {
        
    }
    
    public async Task<IActionResult> OnPostAsync()
    {
        var result = await categoryService.DeleteItemAsync(Id);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message ?? "Не удалось удалить изменения.");
            return Page();
        
        }
            
       return RedirectToPage("/Categories/Index");
    }
    }
}
