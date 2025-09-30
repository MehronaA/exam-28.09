using System;
using Infrastructure.Enum;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/")]
public class ExtraController(IExtraService _service) : ControllerBase
{
    
    
    
    

    

    [HttpGet("dashboard-statistics")]
    public async Task<IActionResult> GetStatistics()
    {
        var result = await _service.DashboardStatistic();

        if (!result.IsSuccess)
            return StatusCode(500, result);

        return Ok(result.Data);
    }

    

    
    
    
}





    


  
    



