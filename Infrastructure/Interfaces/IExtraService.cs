using System;
using Domain.DTOs.Extra;
using Infrastructure.APIResult;

namespace Infrastructure.Interfaces;

public interface IExtraService
{
    Task<Result<DashboardStatisticDto>> DashboardStatistic();


}
